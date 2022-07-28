using PKHeX.Core;
using System.Text;

namespace PokemonCreator
{
    public enum LegalityState
    {
        Legal,
        NeedsRegen,
        Illegal
    }

    public static class PokemonCreatorLegality
    {
        public static async Task<(PKM?, string?)> RegenerateCreationEntity(PokemonCreation pc)
        {
            var entity = pc.Entity;

            // Need to englishlify everything and filter out the illegal stuff
            var englishStrings = GameInfo.GetStrings((int)ProgramLanguage.English);

            var speciesName = englishStrings.Species[entity.Species];
            var itemName = englishStrings.Item[entity.HeldItem];
            var ability = englishStrings.Ability[entity.Ability];
            var nature = englishStrings.Natures[entity.Nature];
            var ball = englishStrings.balllist[entity.Ball];

            var level = pc.LevelRender.ToString();
            var language = (LanguageID)entity.Language;
            var shinyType = pc.ShinyTypeRender;

            var move1 = englishStrings.Move[entity.Move1];
            var move2 = englishStrings.Move[entity.Move2];
            var move3 = englishStrings.Move[entity.Move3];
            var move4 = englishStrings.Move[entity.Move4];

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{speciesName} @ {itemName}");
            sb.AppendLine($"Ability: {ability}");
            sb.AppendLine($"{nature} Nature");
            sb.AppendLine($"Ball: {ball}");
            sb.AppendLine($"Level: {level}");
            sb.AppendLine($"Language: {language}");
            sb.AppendLine($"Shiny: {shinyType}");

            if (pc.FormValueRender != string.Empty)
                sb.AppendLine($".Form={entity.Form}");
            if (pc.FormArgumentRender != string.Empty && entity is IFormArgument ifa)
                sb.AppendLine($".FormArgument={ifa.FormArgument}");

            // Moves to finish off
            sb.AppendLine($"- {move1}");
            sb.AppendLine($"- {move2}");
            sb.AppendLine($"- {move3}");
            sb.AppendLine($"- {move4}");

            var str = sb.ToString();
            var pkm = await PokemonHelper.GenerateSetType(str, pc.PokemonType);
            return (pkm, null);
        }
    }
}
