using Windows.UI;

namespace PaletteNet.Windows
{
    public class ColorConverter
    {
        public static Color ToColor(int color)
        {
            return Color.FromArgb(
                (byte)ColorHelpers.Alpha(color),
                (byte)ColorHelpers.Red(color),
                (byte)ColorHelpers.Green(color),
                (byte)ColorHelpers.Blue(color));
        }

        public static int ToInt(Color color)
        {
            return ColorHelpers.ARGB(color.A, color.R, color.G, color.B);
        }
    }
}
