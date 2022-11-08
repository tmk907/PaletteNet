using Android.Graphics;

namespace PaletteNet.Android
{
    public class ColorConverter
    {
        public static Color ToColor(int color)
        {
            return Color.Argb(
                (byte)ColorHelpers.Alpha(color),
                (byte)ColorHelpers.Red(color),
                (byte)ColorHelpers.Green(color),
                (byte)ColorHelpers.Blue(color));
        }

        public static int ToInt(Color color)
        {
            return color.ToArgb();
        }
    }
}