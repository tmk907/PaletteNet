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

        public ObservableCollection<ColorItem> PaletteColors { get; } = new ObservableCollection<ColorItem>();
        public ObservableCollection<ColorItem> AllColors { get; } = new ObservableCollection<ColorItem>();



        public void CreatePalette(BitmapDecoder decoder)
        {
            PaletteColors.Clear();
            AllColors.Clear();

            var palette = PaletteNet.Windows.PaletteColors.Generate(new BitmapDecoderHelper(decoder));

            PaletteColors.Add(new ColorItem { Color = palette.GetDominantColor(), Description = "Dominant" });
            PaletteColors.Add(new ColorItem { Color = palette.GetLightVibrantColor(), Description = "Light Vibrant" });
            PaletteColors.Add(new ColorItem { Color = palette.GetVibrantColor(), Description = "Vibrant" });
            PaletteColors.Add(new ColorItem { Color = palette.GetDarkVibrantColor(), Description = "Dark Vibrant" });
            PaletteColors.Add(new ColorItem { Color = palette.GetLightMutedColor(), Description = "Light Muted" });
            PaletteColors.Add(new ColorItem { Color = palette.GetMutedColor(), Description = "Muted" });
            PaletteColors.Add(new ColorItem { Color = palette.GetDarkMutedColor(), Description = "Dark Muted" });

            foreach(var c in palette.GetAllColors())
            {
                AllColors.Add(new ColorItem { Color = c});
            }
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
