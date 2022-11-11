using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace PaletteNet.Android
{
    public class PaletteColors
    {
        public int DefaultColor { get; }
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
            DefaultColor = Color.Transparent.ToInt();
        }

        public PaletteColors(Palette palette, Color defaultColor)
        {
            Palette = palette;
            DefaultColor = defaultColor.ToInt();
        }

        public Color DominantColor => (Palette.DominantColor ?? DefaultColor).ToColor();

        public Color VibrantColor => (Palette.VibrantColor ?? DefaultColor).ToColor();

        public Color LightVibrantColor => (Palette.LightVibrantColor ?? DefaultColor).ToColor();

        public Color DarkVibrantColor => (Palette.DarkVibrantColor ?? DefaultColor).ToColor();

        public Color MutedColor => (Palette.MutedColor ?? DefaultColor).ToColor();

        public Color LightMutedColor => (Palette.LightMutedColor ?? DefaultColor).ToColor();

        public Color DarkMutedColor => (Palette.DarkMutedColor ?? DefaultColor).ToColor();

        public IEnumerable<Color> GetAllColors()
        {
            return Palette.Swatches.Select(x => x.Rgb.ToColor());
        }
    }
}
