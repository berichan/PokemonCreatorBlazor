using PKHeX.Core;

namespace PokemonCreator.PokeSprite
{
    public static class SpriteUtil
    {
        public const string SpriteLocaleSpeciesShiny = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.Drawing.PokeSprite/Resources/img/Big%20Shiny%20Sprites/";
        public const string SpriteLocaleSpeciesNormal = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.Drawing.PokeSprite/Resources/img/Big%20Pokemon%20Sprites/";

        public static string GetSpriteName(PKM pk)
        {
            var isFormArg = pk is IFormArgument;
            var spr = SpriteName.GetResourceStringSprite(pk.Species, pk.Form, pk.Gender, isFormArg ? ((IFormArgument)pk).FormArgument : 0, pk.Generation, pk.IsShiny);

            return "b" + spr + ".png";
        }

        public static string GetSpriteURL(PKM pk) => pk.IsShiny ? SpriteLocaleSpeciesShiny + GetSpriteName(pk) : SpriteLocaleSpeciesNormal + GetSpriteName(pk);
    }
}
