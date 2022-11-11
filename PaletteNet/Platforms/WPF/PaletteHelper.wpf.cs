using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PaletteNet.Desktop
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

        public Color GetDominantColor()
        {
            return (Palette.DominantColor ?? DefaultColor).ToColor();
        }

        public Color GetVibrantColor()
        {
            return (Palette.VibrantColor ?? DefaultColor).ToColor();
        }

        public Color GetLightVibrantColor()
        {
            return (Palette.LightVibrantColor ?? DefaultColor).ToColor();
        }

        public Color GetDarkVibrantColor()
        {
            return (Palette.DarkVibrantColor ?? DefaultColor).ToColor();
        }

        public Color GetMutedColor()
        {
            return (Palette.MutedColor ?? DefaultColor).ToColor();
        }

        public Color GetLightMutedColor()
        {
            return (Palette.LightMutedColor ?? DefaultColor).ToColor();
        }

        public Color GetDarkMutedColor()
        {
            return (Palette.DarkMutedColor ?? DefaultColor).ToColor();
        }

        public IEnumerable<Color> GetAllColors()
        {
            return Palette.Swatches.Select(x => x.Rgb.ToColor());
        }
    }
}
