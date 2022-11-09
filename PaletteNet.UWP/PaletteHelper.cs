using Windows.UI;

namespace PaletteNet.UWP
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
            _defaultColor = 0;
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

        public Color GetVibrantColor(Color defaultColor)
        {
            return _palette.GetVibrantColorValue(_defaultColor).ToColor();
        }

        public Color GetLightVibrantColor(Color defaultColor)
        {
            return _palette.GetLightVibrantColorValue(_defaultColor).ToColor();
        }

        public Color GetDarkVibrantColor(Color defaultColor)
        {
            return _palette.GetDarkVibrantColorValue(_defaultColor).ToColor();
        }

        public Color GetMutedColor(Color defaultColor)
        {
            return _palette.GetMutedColorValue(_defaultColor).ToColor();
        }

        public Color GetLightMutedColor(Color defaultColor)
        {
            return _palette.GetLightMutedColorValue(_defaultColor).ToColor();
        }

        public Color GetDarkMutedColor(Color defaultColor)
        {
            return _palette.GetDarkMutedColorValue(_defaultColor).ToColor();
        }

        public IEnumerable<Color> GetAllColors()
        {
            return _palette.GetSwatches().Select(x => x.GetRgb().ToColor());
        }
    }
}
