using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PaletteNet.Windows;
using Windows.Graphics.Imaging;
using Microsoft.UI;
using Windows.UI;
using System.Collections.ObjectModel;

namespace PaletteNetSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
        }

        private Color dominant;
        public Color Dominant
        {
            get { return dominant; }
            set { Set(ref dominant, value); }
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

        public ObservableCollection<ColorItem> PaletteColors { get; } = new ObservableCollection<ColorItem>();

        public void CreatePalette(BitmapDecoder decoder)
        {
            PaletteColors.Clear();

            var palette = PaletteHelper.From(new BitmapDecoderHelper(decoder));

            DarkMuted = palette.GetDarkMutedColor(Colors.Transparent);
            Muted = palette.GetMutedColor(Colors.Transparent);
            LightMuted = palette.GetLightMutedColor(Colors.Transparent);
            DarkVibrant = palette.GetDarkVibrantColor(Colors.Transparent);
            Vibrant = palette.GetVibrantColor(Colors.Transparent);
            LightVibrant = palette.GetLightVibrantColor(Colors.Transparent);
            Dominant = palette.GetDominantColor(Colors.Transparent);

            PaletteColors.Add(new ColorItem { Color = Dominant, Description = "Dominant" });
            PaletteColors.Add(new ColorItem { Color = LightVibrant, Description = "Light Vibrant" });
            PaletteColors.Add(new ColorItem { Color = Vibrant, Description = "Vibrant" });
            PaletteColors.Add(new ColorItem { Color = DarkVibrant, Description = "Dark Vibrant" });
            PaletteColors.Add(new ColorItem { Color = LightMuted, Description = "Light Muted" });
            PaletteColors.Add(new ColorItem { Color = Muted, Description = "Muted" });
            PaletteColors.Add(new ColorItem { Color = DarkMuted, Description = "Dark Muted" });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ColorItem
    {
        public string Description { get; set; }
        public Color Color { get; set; }
    }
}
