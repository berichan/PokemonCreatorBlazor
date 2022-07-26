using PKHeX.Core;
using System.Threading.Tasks;

namespace PokemonCreator
{
    public static class PokemonHelper
    {
        public static async Task<PKM?> GetEmptyPokemon<T>() where T : PKM, new()
        {
            return await GenerateSet<T>("Pikachu\nShiny: Yes");
        }

        public static async Task<PKM?> GenerateSet<T>(string setString) where T : PKM, new()
        {
            var set = new ShowdownSet(setString);
            var template = AutoLegalityWrapper.GetTemplate(set);
            if (set.InvalidLines.Count != 0)
            {
                var msg = $"Unable to parse Showdown Set:\n{string.Join("\n", set.InvalidLines)}";
                Console.WriteLine(msg);
                return null;
            }

            try
            {
                var sav = AutoLegalityWrapper.GetTrainerInfo<T>();
                (var pkm, var result) = await sav.GetLegalAsync(template);
                var la = new LegalityAnalysis(pkm);
                var spec = GameInfo.Strings.Species[template.Species];
                pkm = EntityConverter.ConvertToType(pkm, typeof(T), out _) ?? pkm;
                if (pkm is not T pk || !la.Valid)
                {
                    var reason = result == "Timeout" ? $"That {spec} set took too long to generate." : $"I wasn't able to create a {spec} from that set.";
                    var imsg = $"Oops! {reason}";
                    if (result == "Failed")
                        imsg += $"\n{AutoLegalityWrapper.GetLegalizationHint(template, sav, pkm)}";
                    Console.WriteLine(imsg);
                    return null;
                }
                pk.ResetPartyStats();

                return pk;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Console.WriteLine(ex.Message);
                var msg = $"Oops! An unexpected problem happened with this Showdown Set:\n```{string.Join("\n", set.GetSetLines())}```";
                Console.WriteLine(msg);
            }

            return null;
        }

        public static async Task<PKM?> GetEmptyPokemonType(Type t)
        {
            return await GenerateSetType("Pikachu", t);
        }

        public static async Task<PKM?> GenerateSetType(string setString, Type t)
        {
            var set = new ShowdownSet(setString);
            var template = AutoLegalityWrapper.GetTemplate(set);
            if (set.InvalidLines.Count != 0)
            {
                var msg = $"Unable to parse Showdown Set:\n{string.Join("\n", set.InvalidLines)}";
                Console.WriteLine(msg);
                return null;
            }

            try
            {
                var sav = AutoLegalityWrapper.GetTrainerInfoType(t);
                (var pkm, var result) = await sav.GetLegalAsync(template);
                var la = new LegalityAnalysis(pkm);
                var spec = GameInfo.Strings.Species[template.Species];
                pkm = EntityConverter.ConvertToType(pkm, t, out _) ?? pkm;
                if (pkm.GetType() != t || !la.Valid)
                {
                    var reason = result == "Timeout" ? $"That {spec} set took too long to generate." : $"I wasn't able to create a {spec} from that set.";
                    var imsg = $"Oops! {reason}";
                    if (result == "Failed")
                        imsg += $"\n{AutoLegalityWrapper.GetLegalizationHint(template, sav, pkm)}";
                    Console.WriteLine(imsg);
                    return null;
                }
                pkm.ResetPartyStats();

                return pkm;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Console.WriteLine(ex.Message);
                var msg = $"Oops! An unexpected problem happened with this Showdown Set:\n```{string.Join("\n", set.GetSetLines())}```";
                Console.WriteLine(msg);
            }

            return null;
        }
    }
}
