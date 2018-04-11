using System;
using Windows.UI.Xaml.Media.Imaging;

namespace PaletteNetStandard.UWP
{
    public class WriteableBitmapHelper : IBitmapHelper
    {
        const int DEFAULT_RESIZE_BITMAP_AREA = 112 * 112;
        private WriteableBitmap bitmap;
        private int mResizeArea = DEFAULT_RESIZE_BITMAP_AREA;
        private int mResizeMaxDimension = -1;

        public WriteableBitmapHelper(WriteableBitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public int[] ScaleDownAndGetPixels()
        {
            ScaleBitmapDown();
            byte[] array = bitmap.ToByteArray();
            int[] subsetPixels = new int[bitmap.PixelWidth * bitmap.PixelHeight];

            for (int i = 0; i < subsetPixels.Length - 1; i++)
            {
                subsetPixels[i] = ColorHelpers.ARGB(array[i * 4 + 3], array[i * 4 + 2], array[i * 4 + 1], array[i * 4]);
            }
            return subsetPixels;
        }

        private void ScaleBitmapDown()
        {
            double scaleRatio = -1;

            if (mResizeArea > 0)
            {
                int bitmapArea = bitmap.PixelWidth * bitmap.PixelHeight;
                if (bitmapArea > mResizeArea)
                {
                    scaleRatio = Math.Sqrt(mResizeArea / (double)bitmapArea);
                }
            }
            else if (mResizeMaxDimension > 0)
            {
                int maxDimension = Math.Max(bitmap.PixelWidth, bitmap.PixelHeight);
                if (maxDimension > mResizeMaxDimension)
                {
                    scaleRatio = mResizeMaxDimension / (double)maxDimension;
                }
            }

            if (scaleRatio <= 0)
            {
                // Scaling has been disabled or not needed so just return the WriteableBitmap
                return;
            }

            bitmap = bitmap.Resize(
                    (int)Math.Ceiling(bitmap.PixelWidth * scaleRatio),
                    (int)Math.Ceiling(bitmap.PixelHeight * scaleRatio),
                    WriteableBitmapExtensions.Interpolation.Bilinear);
        }
    }
}
