using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;

namespace PaletteNet.Windows
{
    public class PaletteColors
    {
        public int DefaultColor;

        public Palette Palette { get; }

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
            Palette = palette;
            DefaultColor = 0;
        }

        public PaletteColors(Palette palette, Color defaultColor)
        {
            Palette = palette;
            DefaultColor = defaultColor.ToInt();
        }

        public Color GetDominantColor()
        {
            return Palette.GetDominantColorValue(DefaultColor).ToColor();
        }

        public Color GetVibrantColor()
        {
            return Palette.GetVibrantColorValue(DefaultColor).ToColor();
        }

        public Color GetLightVibrantColor()
        {
            return Palette.GetLightVibrantColorValue(DefaultColor).ToColor();
        }

        public Color GetDarkVibrantColor()
        {
            return Palette.GetDarkVibrantColorValue(DefaultColor).ToColor();
        }

        public Color GetMutedColor()
        {
            return Palette.GetMutedColorValue(DefaultColor).ToColor();
        }

        public Color GetLightMutedColor()
        {
            return Palette.GetLightMutedColorValue(DefaultColor).ToColor();
        }

        public Color GetDarkMutedColor()
        {
            return Palette.GetDarkMutedColorValue(DefaultColor).ToColor();
        }

        public IEnumerable<Color> GetAllColors()
        {
            return Palette.GetSwatches().Select(x => x.GetRgb().ToColor());
        }
    }
}
