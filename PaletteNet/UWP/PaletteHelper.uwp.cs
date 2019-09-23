using Windows.UI;

namespace PaletteNet.UWP
{
    public class PaletteHelper
    {
        public static PaletteColors From(IBitmapHelper bitmapHelper)
        {
            var paletteBuilder = new PaletteBuilder();
            var palette = paletteBuilder.Generate(bitmapHelper);
            return new PaletteColors(palette);
        }
    }

    public class PaletteColors
    {
        private readonly Palette _palette;

        public PaletteColors(Palette palette)
        {
            _palette = palette;
        }

        public Color GetVibrantColor(Color defaultColor)
        {
            return ColorConverter.ToColor(_palette.GetVibrantColorValue(ColorConverter.ToInt(defaultColor)));
        }

        public Color GetLightVibrantColor(Color defaultColor)
        {
            return ColorConverter.ToColor(_palette.GetLightVibrantColorValue(ColorConverter.ToInt(defaultColor)));
        }

        public Color GetDarkVibrantColor(Color defaultColor)
        {
            return ColorConverter.ToColor(_palette.GetDarkVibrantColorValue(ColorConverter.ToInt(defaultColor)));
        }

        public Color GetMutedColor(Color defaultColor)
        {
            return ColorConverter.ToColor(_palette.GetMutedColorValue(ColorConverter.ToInt(defaultColor)));
        }

        public Color GetLightMutedColor(Color defaultColor)
        {
            return ColorConverter.ToColor(_palette.GetLightMutedColorValue(ColorConverter.ToInt(defaultColor)));
        }

        public Color GetDarkMutedColor(Color defaultColor)
        {
            return ColorConverter.ToColor(_palette.GetDarkMutedColorValue(ColorConverter.ToInt(defaultColor)));
        }
    }
}
