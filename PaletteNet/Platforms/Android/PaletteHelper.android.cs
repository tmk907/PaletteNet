using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace PaletteNet.Android
{
    public class PaletteColors
    {
        private readonly Palette _palette;
        private readonly int _defaultColor;

        public static PaletteColors Generate(IBitmapHelper bitmapHelper)
        {
            var paletteBuilder = new PaletteBuilder();
            var palette = paletteBuilder.Generate(bitmapHelper);
            return new PaletteColors(palette);
        }

        public static PaletteColors Generate(IBitmapHelper bitmapHelper, Color defaultColor)
        {
            var paletteBuilder = new PaletteBuilder();
            var palette = paletteBuilder.Generate(bitmapHelper);
            return new PaletteColors(palette, defaultColor);
        }

        public PaletteColors(Palette palette)
        {
            _palette = palette;
            _defaultColor = Color.Transparent.ToInt();
        }

        public PaletteColors(Palette palette, Color defaultColor)
        {
            _palette = palette;
            _defaultColor = defaultColor.ToInt();
        }

        public Color GetDominantColor()
        {
            return _palette.GetDominantColorValue(_defaultColor).ToColor();
        }

        public Color GetVibrantColor()
        {
            return _palette.GetVibrantColorValue(_defaultColor).ToColor();
        }

        public Color GetLightVibrantColor()
        {
            return _palette.GetLightVibrantColorValue(_defaultColor).ToColor();
        }

        public Color GetDarkVibrantColor()
        {
            return _palette.GetDarkVibrantColorValue(_defaultColor).ToColor();
        }

        public Color GetMutedColor()
        {
            return _palette.GetMutedColorValue(_defaultColor).ToColor();
        }

        public Color GetLightMutedColor()
        {
            return _palette.GetLightMutedColorValue(_defaultColor).ToColor();
        }

        public Color GetDarkMutedColor()
        {
            return _palette.GetDarkMutedColorValue(_defaultColor).ToColor();
        }

        public IEnumerable<Color> GetAllColors()
        {
            return _palette.GetSwatches().Select(x => x.GetRgb().ToColor());
        }
    }
}
