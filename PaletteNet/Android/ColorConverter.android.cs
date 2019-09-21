using Android.Graphics;

namespace PaletteNet.Android
{
    public class ColorConverter
    {
        public static Color IntToColor(int color)
        {
            return Color.Argb(
                (byte)ColorHelpers.Alpha(color),
                (byte)ColorHelpers.Red(color),
                (byte)ColorHelpers.Green(color),
                (byte)ColorHelpers.Blue(color));
        }

        public static int ColorToInt(Color color)
        {
            return color.ToArgb();
        }
    }
}