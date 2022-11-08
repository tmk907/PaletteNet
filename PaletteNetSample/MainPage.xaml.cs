using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PaletteNetSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel VM;

        public MainPage()
        {
            this.InitializeComponent();
            VM = new MainPageViewModel();
            this.KeyDown += MainPage_KeyDown;
        }

        private async void MainPage_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Left)
            {
                await PreviousImage();
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                await NextImage();
            }
        }

        private async void OpenImage()
        {
            try
            {
                var picker = new FileOpenPicker();
                InitializeWithWindow.Initialize(picker, MainWindow.Hwnd);

                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add("*");

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    await ChangeImage(file);
                    images.Clear();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private int currentImageIndex = -1;
        private readonly List<string> images = new List<string>();

        private async void OpenFolder()
        {
            try
            {
                var picker = new FolderPicker();
                InitializeWithWindow.Initialize(picker, MainWindow.Hwnd);

                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add("*");

                var folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    images.Clear();
                    images.AddRange(Directory.EnumerateFiles(folder.Path)
                        .Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png"))
                        .OrderBy(x => Path.GetFileName(x)));
                    currentImageIndex = images.Count > 0 ? 0 : -1;
                    await ShowImage(currentImageIndex);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task NextImage()
        {
            currentImageIndex++;
            if (images.Count == 0 || currentImageIndex == images.Count)
            {
                currentImageIndex--;
                return;
            }
            await ShowImage(currentImageIndex);
        }

        private async Task PreviousImage()
        {
            currentImageIndex--;
            if (images.Count == 0 || currentImageIndex < 0)
            {
                currentImageIndex++;
                return;
            }
            await ShowImage(currentImageIndex);
        }

        private async Task ShowImage(int index)
        {
            var file = await StorageFile.GetFileFromPathAsync(images[index]);
            await ChangeImage(file);
        }

        private async Task ChangeImage(StorageFile file)
        {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                Image1.Source = bitmapImage;
            }
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                VM.CreatePalette(decoder);
            }
        }
    }
}
