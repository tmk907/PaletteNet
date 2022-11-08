using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaletteNetSample
{
    public sealed partial class ColorControl : UserControl
    {
        public ColorControl()
        {
            this.InitializeComponent();
            CopyHexCommand = new RelayCommand(CopyHex);
            CopyRGBCommand = new RelayCommand(CopyRGB);
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); UpdateColorNames(); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorControl), new PropertyMetadata(Colors.Transparent));


        public string ColorHex
        {
            get { return (string)GetValue(ColorHexProperty); }
            set { SetValue(ColorHexProperty, value); }
        }

        public static readonly DependencyProperty ColorHexProperty =
            DependencyProperty.Register("ColorHex", typeof(string), typeof(ColorControl), new PropertyMetadata(null));


        public string ColorRGB
        {
            get { return (string)GetValue(ColorRGBProperty); }
            set { SetValue(ColorRGBProperty, value); }
        }

        public static readonly DependencyProperty ColorRGBProperty =
            DependencyProperty.Register("ColorRGB", typeof(string), typeof(ColorControl), new PropertyMetadata(null));

        private void UpdateColorNames()
        {
            if (Color.ToString().StartsWith("#00"))
            {
                ColorHex = "";
                ColorRGB = "";
            }
            else
            {
                ColorHex = $"#{Color.R.ToString("X2")}{Color.G.ToString("X2")}{Color.B.ToString("X2")}";
                ColorRGB = $"({Color.R},{Color.G},{Color.B})";
            }
        }


        public ICommand CopyHexCommand { get; }
        public ICommand CopyRGBCommand { get; }
        
        private void CopyHex()
        {
            CopyToClipboard(ColorHex);
        }

        private void CopyRGB()
        {
            CopyToClipboard(ColorRGB);
        }

        private void CopyToClipboard(string text)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
