using System;
using Android.Graphics;

namespace PaletteNet.Android
{
    public class BitmapHelper : IBitmapHelper
    {
        const int DEFAULT_RESIZE_BITMAP_AREA = 112 * 112;
        private int mResizeArea = DEFAULT_RESIZE_BITMAP_AREA;
        private int mResizeMaxDimension = -1;

        private Bitmap bitmap;

        public BitmapHelper(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public int[] ScaleDownAndGetPixels()
        {
            using var scaledBitmap = ScaleBitmapDown(bitmap);
            if (scaledBitmap == null) return new int[0];

            int bitmapWidth = scaledBitmap.Width;
            int bitmapHeight = scaledBitmap.Height;
            int[] pixels = new int[bitmapWidth * bitmapHeight];
            scaledBitmap.GetPixels(pixels, 0, bitmapWidth, 0, 0, bitmapWidth, bitmapHeight);

            return pixels;
        }

        private Bitmap? ScaleBitmapDown(Bitmap bitmap)
        {
            double scaleRatio = -1;

            if (mResizeArea > 0)
            {
                int bitmapArea = bitmap.Width * bitmap.Height;
                if (bitmapArea > mResizeArea)
                {
                    scaleRatio = Math.Sqrt(mResizeArea / (double)bitmapArea);
                }
            }
            else if (mResizeMaxDimension > 0)
            {
                int maxDimension = Math.Max(bitmap.Width, bitmap.Height);
                if (maxDimension > mResizeMaxDimension)
                {
                    scaleRatio = mResizeMaxDimension / (double)maxDimension;
                }
            }

            if (scaleRatio <= 0)
            {
                // Scaling has been disabled or not needed so just return the Bitmap
                return null;
            }

            return Bitmap.CreateScaledBitmap(bitmap,
                    (int)Math.Ceiling(bitmap.Width * scaleRatio),
                    (int)Math.Ceiling(bitmap.Width * scaleRatio),
                    false);
        }
    }
}