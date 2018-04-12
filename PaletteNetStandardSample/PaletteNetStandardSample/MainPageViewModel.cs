using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using PaletteNetStandard;
using PaletteNetStandard.UWP;
using Windows.Graphics.Imaging;

namespace PaletteNetStandardSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private PaletteBuilder paletteBuilder;
        
        public MainPageViewModel()
        {
            paletteBuilder = new PaletteBuilder();
        }

        private Uri imageUri;
        public Uri ImageUri
        {
            get { return imageUri; }
            set { Set(ref imageUri, value); }
        }

        private Color darkMuted;
        public Color DarkMuted
        {
            get { return darkMuted; }
            set { Set(ref darkMuted, value); }
        }

        private Color muted;
        public Color Muted
        {
            get { return muted; }
            set { Set(ref muted, value); }
        }

        private Color lightMuted;
        public Color LightMuted
        {
            get { return lightMuted; }
            set { Set(ref lightMuted, value); }
        }

        private Color darkVibrant;
        public Color DarkVibrant
        {
            get { return darkVibrant; }
            set { Set(ref darkVibrant, value); }
        }

        private Color vibrant;
        public Color Vibrant
        {
            get { return vibrant; }
            set { Set(ref vibrant, value); }
        }

        private Color lightVibrant;
        public Color LightVibrant
        {
            get { return lightVibrant; }
            set { Set(ref lightVibrant, value); }
        }

        public void CreatePalette(BitmapDecoder decoder)
        {
            IBitmapHelper bitmapHelper = new BtimapDecoderHelper(decoder);
            Palette palette = paletteBuilder.Generate(bitmapHelper);

            DarkMuted = ColorConverter.IntToColor(palette.GetDarkMutedColorValue());
            Muted = ColorConverter.IntToColor(palette.GetMutedColorValue());
            LightMuted = ColorConverter.IntToColor(palette.GetLightMutedColorValue());
            DarkVibrant = ColorConverter.IntToColor(palette.GetDarkVibrantColorValue());
            Vibrant = ColorConverter.IntToColor(palette.GetVibrantColorValue());
            LightVibrant = ColorConverter.IntToColor(palette.GetLightVibrantColorValue());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        public virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
