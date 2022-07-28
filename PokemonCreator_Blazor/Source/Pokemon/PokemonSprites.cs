using System.Runtime.CompilerServices;
using PokemonCreator.PokeSprite;
using PKHeX.Core;

namespace PokemonCreator
{ 
    public static class PokemonSprites
    {
        private const string SpriteLocaleItems = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.Drawing.PokeSprite/Resources/img/Big%20Items/";
        private const string SpriteLocaleBalls = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.Drawing.PokeSprite/Resources/img/ball/";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PokemonSpeciesSprite(int dexNum) => $"{SpriteUtil.SpriteLocaleSpeciesNormal}b_{dexNum}{(dexNum != (int)Species.Alcremie ? string.Empty : "-0-0")}.png";
        public static string PokemonSpeciesSpriteHTML(int dexNum) => $"<img src=\"{PokemonSpeciesSprite(dexNum)}\" style=\"width:42.5px;height:35px;\"/>";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PokemonItemSprite(int itemNum) => $"{SpriteLocaleItems}bitem_{itemNum}.png";
        public static string PokemonItemSpriteHTML(int itemNum) => $"<img src=\"{PokemonItemSprite(itemNum)}\" style=\"width:20px;height:20px;\"/>";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PokemonBallSprite(int ballNum) => $"{SpriteLocaleBalls}_ball{ballNum}.png";
        public static string PokemonBallSpriteHTML(int ballNum) => $"<img src=\"{PokemonBallSprite(ballNum)}\" style=\"width:20px;height:20px;\"/>";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PokemonEntitySprite(PKM pk) => SpriteUtil.GetSpriteURL(pk);
        public static string PokemonEntitySpriteHTML(PKM? pk, bool large = false)
        {
            var w = large ? "85" : "42.5";
            var h = large ? "70" : "35";
            var v = pk == null ? PokemonSpeciesSpriteHTML(1) : $"<img src=\"{SpriteUtil.GetSpriteURL(pk)}\" style=\"width:{w}px;height:{h}px;\"/>";
            return v;
        }
    }
}
