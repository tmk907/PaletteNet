using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PaletteNet.Windows;
using Windows.Graphics.Imaging;
using Windows.UI;
using System.Collections.ObjectModel;
using PaletteNet;

namespace PaletteNetSample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ColorItem> PaletteColors { get; } = new ObservableCollection<ColorItem>();
        public ObservableCollection<ColorItem> AllColors { get; } = new ObservableCollection<ColorItem>();

        public void CreatePalette(BitmapDecoder decoder)
        {
            PaletteColors.Clear();
            AllColors.Clear();

            var palette = PaletteNet.Windows.PaletteColors.Generate(new BitmapDecoderHelper(decoder));

            PaletteColors.Add(CreateColorItem(palette.Palette.DominantSwatch, "Dominant"));
            PaletteColors.Add(CreateColorItem(palette.Palette.LightVibrantSwatch, "Light Vibrant"));
            PaletteColors.Add(CreateColorItem(palette.Palette.VibrantSwatch, "Vibrant"));
            PaletteColors.Add(CreateColorItem(palette.Palette.DarkVibrantSwatch, "Dark Vibrant"));
            PaletteColors.Add(CreateColorItem(palette.Palette.LightMutedSwatch, "Light Muted"));
            PaletteColors.Add(CreateColorItem(palette.Palette.MutedSwatch, "Muted"));
            PaletteColors.Add(CreateColorItem(palette.Palette.DarkMutedSwatch, "Dark Muted"));

            foreach(var swatch in palette.Palette.Swatches)
            {
                AllColors.Add(CreateColorItem(swatch, ""));
            }
        }

        private ColorItem CreateColorItem(Swatch swatch, string description)
        {
            return new ColorItem
            {
                Color = (swatch?.Rgb ?? 0).ToColor(),
                TitleColor = (swatch?.TitleTextColor ?? 0).ToColor(),
                BodyColor = (swatch?.BodyTextColor ?? 0).ToColor(),
                Description = description
            };
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
        public Color TitleColor { get; set; }
        public Color BodyColor { get; set; }

    }
}
