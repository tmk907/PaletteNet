using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace PaletteNet.Windows
{
    public class BitmapDecoderHelper : IBitmapHelper
    {
        const int DEFAULT_RESIZE_BITMAP_AREA = 112 * 112;
        private int mResizeArea = DEFAULT_RESIZE_BITMAP_AREA;
        private int mResizeMaxDimension = -1;

        private readonly BitmapDecoder _decoder;

        public BitmapDecoderHelper(BitmapDecoder decoder)
        {
            this._decoder = decoder;
        }

        public int[] ScaleDownAndGetPixels()
        {
            byte[] pixels = Task.Run(() => ScaleBitmapDown()).Result;
            int[] subsetPixels = new int[pixels.Length / 4];

            for (int i = 0; i < subsetPixels.Length - 1; i++)
            {
                subsetPixels[i] = ColorHelpers.ARGB(pixels[i * 4 + 3], pixels[i * 4 + 2], pixels[i * 4 + 1], pixels[i * 4]);
            }
            return subsetPixels;
        }

        private async Task<byte[]> ScaleBitmapDown()
        {
            double scaleRatio = -1;

            if (mResizeArea > 0)
            {
                uint bitmapArea = _decoder.PixelWidth * _decoder.PixelHeight;
                if (bitmapArea > mResizeArea)
                {
                    scaleRatio = Math.Sqrt(mResizeArea / (double)bitmapArea);
                }
            }
            else if (mResizeMaxDimension > 0)
            {
                int maxDimension = Math.Max((int)_decoder.PixelWidth, (int)_decoder.PixelHeight);
                if (maxDimension > mResizeMaxDimension)
                {
                    scaleRatio = mResizeMaxDimension / (double)maxDimension;
                }
            }

            PixelDataProvider pixelsData;

            if (scaleRatio > 0)
            {
                BitmapTransform bt = new BitmapTransform();
                bt.ScaledHeight = (uint)Math.Ceiling(_decoder.PixelHeight * scaleRatio);
                bt.ScaledWidth = (uint)Math.Ceiling(_decoder.PixelWidth * scaleRatio);
                pixelsData = await _decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight, bt,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);
            }
            else
            {
                pixelsData = await _decoder.GetPixelDataAsync();
            }
            return pixelsData.DetachPixelData();
        }
    }
}
