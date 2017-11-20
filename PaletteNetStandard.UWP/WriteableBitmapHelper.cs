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

        public int[] GetPixelsFromBitmap()
        {
            byte[] array = bitmap.ToByteArray();
            int[] subsetPixels = new int[bitmap.PixelWidth * bitmap.PixelHeight];

            for (int i = 0; i < subsetPixels.Length - 1; i++)
            {
                subsetPixels[i] = ColorHelpers.ARGB(array[i * 4 + 3], array[i * 4 + 2], array[i * 4 + 1], array[i * 4]);
            }
            return subsetPixels;
        }

        public void ScaleBitmapDown()
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


        /**
        * Set the resize value when using a {@link android.graphics.WriteableBitmap} as the source.
        * If the bitmap's largest dimension is greater than the value specified, then the bitmap
        * will be resized so that its largest dimension matches {@code maxDimension}. If the
        * bitmap is smaller or equal, the original is used as-is.
        *
        * //@Deprecated Using {@link #resizeWriteableBitmapArea(int)} is preferred since it can handle
        * abnormal aspect ratios more gracefully.
        *
        * @param maxDimension the number of pixels that the max dimension should be scaled down to,
        *                     or any value <= 0 to disable resizing.
        */
        //@Deprecated
        public void SetBitmapSize(int maxDimension)
        {
            mResizeMaxDimension = maxDimension;
            mResizeArea = -1;
        }

        /**
         * Set the resize value when using a {@link android.graphics.WriteableBitmap} as the source.
         * If the bitmap's area is greater than the value specified, then the bitmap
         * will be resized so that its area matches {@code area}. If the
         * bitmap is smaller or equal, the original is used as-is.
         * <p>
         * This value has a large effect on the processing time. The larger the resized image is,
         * the greater time it will take to generate the palette. The smaller the image is, the
         * more detail is lost in the resulting image and thus less precision for color selection.
         *
         * @param area the number of pixels that the intermediary scaled down WriteableBitmap should cover,
         *             or any value <= 0 to disable resizing.
         */
        public void SetBitmapArea(int area)
        {
            mResizeArea = area;
            mResizeMaxDimension = -1;
        }
    }
}
