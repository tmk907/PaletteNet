using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PaletteNetStandard.Android
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
            ScaleBitmapDown(bitmap);
            int bitmapWidth = bitmap.Width;
            int bitmapHeight = bitmap.Height;
            int[] pixels = new int[bitmapWidth * bitmapHeight];
            bitmap.GetPixels(pixels, 0, bitmapWidth, 0, 0, bitmapWidth, bitmapHeight);

            return pixels;
        }

        private void ScaleBitmapDown(Bitmap bitmap)
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
                return bitmap;
            }

            return Bitmap.CreateScaledBitmap(bitmap,
                    (int)Math.Ceiling(bitmap.Width * scaleRatio),
                    (int)Math.Ceiling(bitmap.Width * scaleRatio),
                    false);
        }
    }
}