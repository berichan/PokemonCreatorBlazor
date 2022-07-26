using System.Runtime.CompilerServices;

namespace PokemonCreator
{ 
    public static class PokemonSprites
    {
        private const string SpriteLocaleSpeciesShiny = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.Drawing.PokeSprite/Resources/img/Big%20Shiny%20Sprites/";
        private const string SpriteLocaleItems = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.Drawing.PokeSprite/Resources/img/Big%20Items/";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PokemonBaseSprite(int dexNum) => $"{SpriteLocaleSpeciesShiny}b_{dexNum}s.png";
        public static string PokemonBaseSpriteHTML(int dexNum) => $"<img src=\"{PokemonBaseSprite(dexNum)}\" style=\"width:42.5px;height:35px;\"/>";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PokemonItemSprite(int itemNum) => $"{SpriteLocaleItems}bitem_{itemNum}.png";
        public static string PokemonItemSpriteHTML(int itemNum) => $"<img src=\"{PokemonItemSprite(itemNum)}\" style=\"width:20px;height:20px;\"/>";

    }
}
