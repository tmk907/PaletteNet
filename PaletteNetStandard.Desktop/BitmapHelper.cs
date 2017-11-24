using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PaletteNetStandard.Desktop
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

        public int[] GetPixelsFromBitmap()
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            bitmap.UnlockBits(bitmapData);
            int[] subsetPixels = new int[bitmap.Width * bitmap.Height];
            for (int i = 0; i < subsetPixels.Length - 1; i++)
            {
                subsetPixels[i] = ColorHelpers.ARGB(pixels[i * 4 + 3], pixels[i * 4 + 2], pixels[i * 4 + 1], pixels[i * 4]);
            }
            return subsetPixels;
        }

        public void ScaleBitmapDown()
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
                // Scaling has been disabled or not needed so just return the WriteableBitmap
                return;
            }

            bitmap = new Bitmap(bitmap, new Size(
                    (int)Math.Ceiling(bitmap.Width * scaleRatio),
                    (int)Math.Ceiling(bitmap.Height * scaleRatio)
                ));
        }
    }
}
