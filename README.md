# PaletteNetStandard
.NET port of Android's Palette https://developer.android.com/reference/android/support/v7/graphics/Palette.html

## Example
```c#
using PaletteNetStandard;
using PaletteNetStandard.UWP;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;

public class Example
{
    public Color GetDarkMutedColor(WriteableBitmap bitmap)
    {
        IBitmapHelper bitmapHelper = new WriteableBitmapHelper(bitmap);
        Palette palette = Palette.From(bitmapHelper).Generate();
        Color color = ColorConverter.IntToColor(palette.GetDarkMutedColor());
        return color;
    }
}
```
