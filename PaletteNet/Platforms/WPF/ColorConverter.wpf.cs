using System.Drawing;

namespace PaletteNet.Desktop
{
    public static class ColorExtensions
    {
        public static Color ToColor(this int color)
        {
            return Color.FromArgb(
                (byte)ColorHelpers.Alpha(color),
                (byte)ColorHelpers.Red(color),
                (byte)ColorHelpers.Green(color),
                (byte)ColorHelpers.Blue(color));
        }

        public static int ToInt(this Color color)
        {
            return ColorHelpers.ARGB(color.A, color.R, color.G, color.B);
        }
    }
}
