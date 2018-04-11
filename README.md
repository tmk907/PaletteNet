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

![alt text](https://github.com/tmk907/PaletteNetStandard/blob/master/images/example1.jpg "Example 1")
![alt text](https://github.com/tmk907/PaletteNetStandard/blob/master/images/example2.jpg "Example 2")
