using System.Drawing;

namespace PaletteNet.Desktop
{
    public class ColorConverter
    {
        public static Color IntToColor(int color)
        {
            return Color.FromArgb(
                (byte)ColorHelpers.Alpha(color),
                (byte)ColorHelpers.Red(color),
                (byte)ColorHelpers.Green(color),
                (byte)ColorHelpers.Blue(color));
        }

        public static int ColorToInt(Color color)
        {
            return ColorHelpers.ARGB(color.A, color.R, color.G, color.B);
        }
    }
}
