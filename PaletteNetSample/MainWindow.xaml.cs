using Microsoft.UI.Xaml;
using System;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaletteNetSample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            rootFrame.Navigate(typeof(MainPage));
        }

        public static IntPtr Hwnd;
    }
}
