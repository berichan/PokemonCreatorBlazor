using Microsoft.AspNetCore.Components;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace PokemonCreator
{
    public partial class PokemonCreation : ComponentBase
    {
        public Type PokemonType { get; set; } = null!;
        public PKM Entity { get; set; } = null!;
        public SaveFile BlankSave { get; set; } = null!;
        public ProgramLanguage SiteLanguage { get; set; } = ProgramLanguage.English;

        #region Savedata-specific vars
        public bool CanHoldItems { get; set; }
        #endregion

        #region Freeflow strings to render
        public string PokemonNameRender { get; set; }
        public string PokemonSpeciesRender { get; set; }
        public string PokemonItemRender { get; set; }
        public string PokemonAbilityRender { get; set; }

        public string FormValueRender { get; set; } = string.Empty;
        public string FormArgumentRender { get; set; } = string.Empty;

        public string Move1Render { get; set; }
        public string Move2Render { get; set; }
        public string Move3Render { get; set; }
        public string Move4Render { get; set; }

        public string NatureRender { get; set; }

        public string ShinyTypeRender { get; set; }
        public string BallRender { get; set; } 

        #endregion

        #region Freeflow numerical values
        public int LevelRender { get; set; } = 1;

        public int IV_HP { get; set; }
        public int IV_ATK { get; set; }
        public int IV_DEF { get; set; }
        public int IV_SPE { get; set; }
        public int IV_SPA { get; set; }
        public int IV_SPD { get; set; }

        public int EV_HP { get; set; }
        public int EV_ATK { get; set; }
        public int EV_DEF { get; set; }
        public int EV_SPE { get; set; }
        public int EV_SPA { get; set; }
        public int EV_SPD { get; set; }
        public int SumEV { get => EV_HP + EV_ATK + EV_DEF + EV_SPE + EV_SPA + EV_SPD; }
        #endregion

        [Inject]
        public IFileReaderService fileReaderService { get; set; }
        public ElementReference inputTypeFileElement;

        protected override async Task OnInitializedAsync()
        {
            AutoLegalityWrapper.EnsureInitialized(new LegalitySettings());
            APILegality.PrioritizeGameVersion = (GameVersion)AutoLegalityWrapper.GetTrainerInfoType(PokemonType).Game;

            var poke = await PokemonHelper.GetEmptyPokemonType(PokemonType);
            if (poke != null)
                Entity = poke;

            Console.WriteLine($"Default Pokemon set to: {(Species)Entity.Species} from version {(GameVersion)Entity.Version}");

            BlankSave = SaveUtil.GetBlankSAV(Entity.Context.GetSingleGameVersion(), "beri");
            RecentTrainerCache.SetRecentTrainer(BlankSave);

            CanHoldItems = BlankSave is not SAV8LA;

            GameInfo.FilteredSources = new FilteredGameDataSource(BlankSave, GameInfo.Sources, false);
            var sources = GameInfo.FilteredSources;
            Console.WriteLine(sources.Moves.Count);
            LocalizeUtil.InitializeStrings(GameLanguage.Language2Char((int)SiteLanguage), BlankSave, false);
            PopulateFilteredDataSources(BlankSave);

            await OnEntityChanged();
            RenderCurrentPokemon();
        }

        public async Task ReadFile()
        {
            foreach (var file in await fileReaderService.CreateReference(inputTypeFileElement).EnumerateFilesAsync())
            {
                var info = await file.ReadFileInfoAsync();

                if (info == null || info.Size >= 1024)
                    continue;

                // Read file fully into memory and act
                using (MemoryStream memoryStream = await file.CreateMemoryStreamAsync((int)info.Size))
                {
                    var buffer = new byte[info.Size];

                    // Sync calls are ok once file is in memory
                    memoryStream.Read(buffer);

                    // Check legality
                    var prefer = EntityFileExtension.GetContextFromExtension(info.Name, EntityContext.None);
                    var pkm = EntityFormat.GetFromBytes(buffer, prefer);
                    if (pkm == null)
                    {
                        Console.WriteLine($"{info.Name} is invalid.");
                        continue;
                    }
                    else
                    {
                        var legality = new LegalityAnalysis(pkm, BlankSave.Personal);
                        if (legality.Parsed && legality.Valid)
                        {
                            var conversion = EntityConverter.ConvertToType(pkm, PokemonType, out var res);
                            if (conversion != null)
                            {
                                Entity = conversion;
                                RenderCurrentPokemon();
                            }
                        }
                        else
                            Console.WriteLine($"{info.Name} is valid. This is a {(Species)pkm.Species}");
                    }
                }
            }
        }

        // PKHeX Winforms Blazor reimpl

        protected readonly LegalMoveSource<ComboItem> LegalMoveSource = new(new LegalMoveComboSource());

        #region filters
        public string SpeciesFilter { get; set; } = string.Empty;
        public string HeldItemFilter { get; set; } = string.Empty;
        #endregion

        public List<ComboItem> Species = new();
        public List<ComboItem> Language = new();
        public List<ComboItem> Items = new();
        public List<ComboItem> Balls = new();
        public List<ComboItem> Games = new();
        public List<ComboItem> Abilities = new();
        public List<ComboItem> Natures = new();
        public List<ComboItem> HTLanguage = new();
        public List<ComboItem> BattleVersion = new();

        public List<ComboItem> Moves1 = new();
        public List<ComboItem> Moves2 = new();
        public List<ComboItem> Moves3 = new();
        public List<ComboItem> Moves4 = new();

        // On-the-fly
        public List<ComboItem> AbilityChoices = new();

        public List<string> Forms { get; set; } = new();
        public List<string> FormArgs { get; set; } = new();

        private static bool IsDifferentCount(IReadOnlyCollection<ComboItem> update, List<ComboItem> exist, bool force = false)
        {
            return !(!force && exist.Count == update.Count);
        }

        private void PopulateFilteredDataSources(ITrainerInfo sav, bool force = false)
        {
            var source = GameInfo.FilteredSources;
            if (IsDifferentCount(source.Languages, Language, force))
                Language = new(source.Languages);

            if (sav.Generation >= 2)
            {
                var game = (GameVersion)sav.Game;
                if (game <= 0)
                    game = Entity.Context.GetSingleGameVersion();
                if (IsDifferentCount(source.Items, Items, force))
                    Items = new(source.Items);
            }

            if (sav.Generation >= 3)
            {
                if (IsDifferentCount(source.Balls, Balls, force))
                    Balls = new(source.Balls);
                if (IsDifferentCount(source.Games, Games, force))
                    Games = new(source.Games);
            }

            if (sav.Generation >= 4)
                if (IsDifferentCount(source.Abilities, Abilities, force))
                    Abilities = new(source.Abilities);

            if (sav.Generation >= 8)
            {
                var lang = source.Languages;
                var langWith0 = new List<ComboItem>(1 + lang.Count) { GameInfo.Sources.Empty };
                langWith0.AddRange(lang);
                if (IsDifferentCount(langWith0, HTLanguage, force))
                    HTLanguage = new(langWith0);

                var game = source.Games;
                var gamesWith0 = new List<ComboItem>(1 + game.Count) { GameInfo.Sources.Empty };
                gamesWith0.AddRange(game);
                if (IsDifferentCount(gamesWith0, BattleVersion, force))
                    BattleVersion = new(gamesWith0);
            }
            if (IsDifferentCount(source.Species, Species, force))
                Species = new(source.Species);
            Species = Species.OrderBy(x => x.Value).ToList();

            // Filter out swsh species
            if (PokemonType == typeof(PK8))
                Species = Species.Where(x => SimpleEdits.ExistsInGame(GameVersion.SW, x.Value, 0) || SimpleEdits.ExistsInGame(GameVersion.SH, x.Value, 0)).ToList();

            // Set the Move ComboBoxes too..
            LegalMoveSource.ChangeMoveSource(source.Moves);

            // All moves are the same
            if (IsDifferentCount(source.Moves, Moves1, force))
            {
                Moves1 = new(source.Moves);
                Moves2 = new(source.Moves);
                Moves3 = new(source.Moves);
                Moves4 = new(source.Moves);
            }

            Natures = new(source.Natures);
            //if (sav is SAV8LA)
            //    SetIfDifferentCount(source.Moves, CB_AlphaMastered, force);
        }

        public IEnumerable<ComboItem> GetLegalMoves()
        {
            var AllMovesPlusLegal = LegalMoveSource.Display.DataSource.Where(x => LegalMoveSource.Info.CanLearn(x.Value) && !Entity.Moves.Contains(x.Value));
            return AllMovesPlusLegal.TakeWhile(x => x.Value != 0);
        }

        public void BindNumerics()
        {
            Entity.IV_ATK = IV_ATK;
            Entity.IV_DEF = IV_DEF;
            Entity.IV_HP = IV_HP;
            Entity.IV_SPA = IV_SPA;
            Entity.IV_SPD = IV_SPD;
            Entity.IV_SPE = IV_SPE;

            Entity.EV_ATK = EV_ATK;
            Entity.EV_DEF = EV_DEF;
            Entity.EV_HP = EV_HP;
            Entity.EV_SPA = EV_SPA;
            Entity.EV_SPD = EV_SPD;
            Entity.EV_SPE = EV_SPE;
        }

        public async Task OnNewEntityRequested()
        {
            var newEntityName = GameInfo.GetStrings((int)ProgramLanguage.English).Species[Entity.Species];
            var newEntity = await PokemonHelper.GenerateSetType(newEntityName, PokemonType);
            if (newEntity != null)
                Entity = newEntity;

            await OnEntityChanged();
        }

        public async Task OnEntityChanged()
        {
            RenderForm();
        }

        public void RenderCurrentPokemon()
        {
            // Update all
            var Legality = new LegalityAnalysis(Entity, BlankSave.Personal);
            if (Legality.Parsed)
                LegalMoveSource.ReloadMoves(Legality.GetSuggestedMovesAndRelearn());

            AbilityChoices = GameInfo.FilteredSources.GetAbilityList(Entity).ToList();

            // Form
            if (FormValueRender != string.Empty)
            {
                Entity.Form = Forms.IndexOf(FormValueRender);
                if (FormArgumentRender != string.Empty && Entity is IFormArgument ifa)
                {
                    var arg = (uint)FormArgs.IndexOf(FormArgumentRender);
                    ifa.ChangeFormArgument(Entity.SpecForm, Entity.Form, Entity.Generation, arg);
                }
            }

            // Update renders
            PokemonSpeciesRender = GameInfo.Strings.Species[Entity.Species];
            PokemonNameRender = PokemonSpeciesRender + " " + PokemonSprites.PokemonEntitySpriteHTML(Entity);
            PokemonItemRender = GameInfo.Strings.Item[Entity.HeldItem] + " " + PokemonSprites.PokemonItemSpriteHTML(Entity.HeldItem);
            PokemonAbilityRender = GameInfo.Strings.Ability[Entity.Ability];

            // Shiny
            var ShinyState = ShinyExtensions.GetType(Entity);
            if (ShinyState != Shiny.Never)
            {
                if (ShinyState == Shiny.AlwaysStar)
                    ShinyTypeRender = PokemonHelper.ShinyTypes[1];
                if (ShinyState == Shiny.AlwaysSquare)
                    ShinyTypeRender = PokemonHelper.ShinyTypes[2];
            }
            else
                ShinyTypeRender = PokemonHelper.ShinyTypes[0];

            BallRender = GameInfo.Strings.balllist[Entity.Ball];

            // Moves
            Move1Render = GameInfo.Strings.Move[Entity.Move1];
            Move2Render = GameInfo.Strings.Move[Entity.Move2];
            Move3Render = GameInfo.Strings.Move[Entity.Move3];
            Move4Render = GameInfo.Strings.Move[Entity.Move4];

            NatureRender = GameInfo.Strings.Natures[Entity.Nature];
            LevelRender = Entity.CurrentLevel;

            // IVs
            IV_ATK = Entity.IV_ATK;
            IV_HP = Entity.IV_HP;
            IV_SPD = Entity.IV_SPD;
            IV_SPE = Entity.IV_SPE;
            IV_DEF = Entity.IV_DEF;
            IV_SPA = Entity.IV_SPA;

            // EVs
            EV_ATK = Entity.EV_ATK;
            EV_HP = Entity.EV_HP;
            EV_SPD = Entity.EV_SPD;
            EV_SPE = Entity.EV_SPE;
            EV_DEF = Entity.EV_DEF;
            EV_SPA = Entity.EV_SPA;
        }

        private void RenderForm()
        {
            int species = Entity.Species;
            var pi = BlankSave.Personal[species];
            bool hasForms = FormInfo.HasFormSelection(pi, species, Entity.Format);

            if (!hasForms)
            {
                FormArgumentRender = string.Empty;
                FormValueRender = string.Empty;
                Forms.Clear();
                FormArgs.Clear();
                return;
            }

            var ds = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, Entity.Format);
            if (ds.Length == 1 && string.IsNullOrEmpty(ds[0])) // empty (Alolan Totems)
            {
                FormValueRender = string.Empty;
                Forms.Clear();
            }
            else
            {
                Forms = new(ds);
                FormValueRender = Forms[Entity.Form];
            }

            var max = FormArgumentUtil.GetFormArgumentMax(species, Entity.Form, Entity.Generation);
            if (max != 0 && Entity is IFormArgument ifa)
            {
                bool named = FormConverter.GetFormArgumentIsNamedIndex(species);
                if (named)
                    FormArgs = new(FormConverter.GetFormArgumentStrings(species));

                FormArgumentRender = FormArgs[(int)ifa.FormArgument];
            }
            else
            {
                FormArgumentRender = string.Empty;
                FormArgs.Clear();
            }
        }
    

    }
}
