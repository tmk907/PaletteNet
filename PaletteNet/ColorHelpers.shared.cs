using System;

namespace PaletteNet
{
    public class ColorHelpers
    {
        private const int MIN_ALPHA_SEARCH_MAX_ITERATIONS = 10;
        private const int MIN_ALPHA_SEARCH_PRECISION = 10;
        public const int BLACK = -16777216;
        public const int WHITE = -1;

        public static int Red(int color)
        {
            return (color >> 16) & 0xff;
        }

        public static int Green(int color)
        {
            return (color >> 8) & 0xff;
        }

        public static int Blue(int color)
        {
            return (color) & 0xff;
        }

        public static int Alpha(int color)
        {
            return (color >> 24) & 0xff;
        }

        public static int RGB(int r, int g, int b)
        {
            r = (r << 16) & 0x00FF0000; //Shift red 16-bits and mask out other stuff
            g = (g << 8) & 0x0000FF00; //Shift Green 8-bits and mask out other stuff
            b = b & 0x000000FF; //Mask out anything not blue.

            //0xFF000000 for 100% Alpha. Bitwise OR everything together.
            return (int)(0xFF000000 | (uint)r | (uint)g | (uint)b);
        }

        public static int ARGB(int a, int r, int g, int b)
        {
            return (a & 0xff) << 24 | (r & 0xff) << 16 | (g & 0xff) << 8 | (b & 0xff);
        }

        public static void ColorToHSL(int color, float[] hsl)
        {
            RGBToHSL(Red(color), Green(color), Blue(color), hsl);
        }

        public static void RGBToHSL(int r, int g, int b, float[] hsl)
        {
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;
            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float deltaMaxMin = max - min;
            float h, s;
            float l = (max + min) / 2f;
            if (max == min)
            {
                // Monochromatic
                h = s = 0f;
            }
            else
            {
                if (max == rf)
                {
                    h = ((gf - bf) / deltaMaxMin) % 6f;
                }
                else if (max == gf)
                {
                    h = ((bf - rf) / deltaMaxMin) + 2f;
                }
                else
                {
                    h = ((rf - gf) / deltaMaxMin) + 4f;
                }
                s = deltaMaxMin / (1f - Math.Abs(2f * l - 1f));
            }
            hsl[0] = (h * 60f) % 360f;
            hsl[1] = s;
            hsl[2] = l;
        }

        public static int HSLToColor(float[] hsl)
        {
            float h = hsl[0];
            float s = hsl[1];
            float l = hsl[2];
            float c = (1f - Math.Abs(2 * l - 1f)) * s;
            float m = l - 0.5f * c;
            float x = c * (1f - Math.Abs((h / 60f % 2f) - 1f));
            int hueSegment = (int)h / 60;
            int r = 0, g = 0, b = 0;
            switch (hueSegment)
            {
                case 0:
                    r = (int)Math.Round(255 * (c + m));
                    g = (int)Math.Round(255 * (x + m));
                    b = (int)Math.Round(255 * m);
                    break;
                case 1:
                    r = (int)Math.Round(255 * (x + m));
                    g = (int)Math.Round(255 * (c + m));
                    b = (int)Math.Round(255 * m);
                    break;
                case 2:
                    r = (int)Math.Round(255 * m);
                    g = (int)Math.Round(255 * (c + m));
                    b = (int)Math.Round(255 * (x + m));
                    break;
                case 3:
                    r = (int)Math.Round(255 * m);
                    g = (int)Math.Round(255 * (x + m));
                    b = (int)Math.Round(255 * (c + m));
                    break;
                case 4:
                    r = (int)Math.Round(255 * (x + m));
                    g = (int)Math.Round(255 * m);
                    b = (int)Math.Round(255 * (c + m));
                    break;
                case 5:
                case 6:
                    r = (int)Math.Round(255 * (c + m));
                    g = (int)Math.Round(255 * m);
                    b = (int)Math.Round(255 * (x + m));
                    break;
            }
            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));
            return RGB(r, g, b);
        }

        public static int CalculateMinimumAlpha(int foreground, int background, float minContrastRatio)
        {
            if (Alpha(background) != 255)
            {
                throw new Exception("background can not be translucent");
            }
            // First lets check that a fully opaque foreground has sufficient contrast
            int testForeground = SetAlphaComponent(foreground, 255);
            double testRatio = CalculateContrast(testForeground, background);
            if (testRatio < minContrastRatio)
            {
                // Fully opaque foreground does not have sufficient contrast, return error
                return -1;
            }
            // Binary search to find a value with the minimum value which provides sufficient contrast
            int numIterations = 0;
            int minAlpha = 0;
            int maxAlpha = 255;
            while (numIterations <= MIN_ALPHA_SEARCH_MAX_ITERATIONS &&
                    (maxAlpha - minAlpha) > MIN_ALPHA_SEARCH_PRECISION)
            {
                int testAlpha = (minAlpha + maxAlpha) / 2;
                testForeground = SetAlphaComponent(foreground, testAlpha);
                testRatio = CalculateContrast(testForeground, background);
                if (testRatio < minContrastRatio)
                {
                    minAlpha = testAlpha;
                }
                else
                {
                    maxAlpha = testAlpha;
                }
                numIterations++;
            }
            // Conservatively return the max of the range of possible alphas, which is known to pass.
            return maxAlpha;
        }

        public static double CalculateContrast(int foreground, int background)
        {
            if (Alpha(background) != 255)
            {
                throw new Exception("background can not be translucent");
            }
            if (Alpha(foreground) < 255)
            {
                // If the foreground is translucent, composite the foreground over the background
                foreground = CompositeColors(foreground, background);
            }
            double luminance1 = CalculateLuminance(foreground) + 0.05;
            double luminance2 = CalculateLuminance(background) + 0.05;
            // Now return the lighter luminance divided by the darker luminance
            return Math.Max(luminance1, luminance2) / Math.Min(luminance1, luminance2);
        }

        public static double CalculateLuminance(int color)
        {
            double red = Red(color) / 255d;
            red = red < 0.03928 ? red / 12.92 : Math.Pow((red + 0.055) / 1.055, 2.4);
            double green = Green(color) / 255d;
            green = green < 0.03928 ? green / 12.92 : Math.Pow((green + 0.055) / 1.055, 2.4);
            double blue = Blue(color) / 255d;
            blue = blue < 0.03928 ? blue / 12.92 : Math.Pow((blue + 0.055) / 1.055, 2.4);
            return (0.2126 * red) + (0.7152 * green) + (0.0722 * blue);
        }

        public static int CompositeColors(int foreground, int background)
        {
            int bgAlpha = Alpha(background);
            int fgAlpha = Alpha(foreground);
            int a = CompositeAlpha(fgAlpha, bgAlpha);
            int r = CompositeComponent(Red(foreground), fgAlpha,
                    Red(background), bgAlpha, a);
            int g = CompositeComponent(Green(foreground), fgAlpha,
                    Green(background), bgAlpha, a);
            int b = CompositeComponent(Blue(foreground), fgAlpha,
                    Blue(background), bgAlpha, a);
            return ARGB(a, r, g, b);
        }

        public static int CompositeAlpha(int foregroundAlpha, int backgroundAlpha)
        {
            return 0xFF - (((0xFF - backgroundAlpha) * (0xFF - foregroundAlpha)) / 0xFF);
        }

        private static int CompositeComponent(int fgC, int fgA, int bgC, int bgA, int a)
        {
            if (a == 0) return 0;
            return ((0xFF * fgC * fgA) + (bgC * bgA * (0xFF - fgA))) / (a * 0xFF);
        }

        public static int SetAlphaComponent(int color, int alpha)
        {
            if (alpha < 0 || alpha > 255)
            {
                throw new Exception("alpha must be between 0 and 255.");
            }
            return (color & 0x00ffffff) | (alpha << 24);
        }
    }
}
