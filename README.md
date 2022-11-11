# PaletteNet
Library to extract prominent colors from an image. 
- Dominant
- Vibrant
- Vibrant Dark
- Vibrant Light
- Muted
- Muted Dark
- Muted Light  

.NET port of Android's Palette https://developer.android.com/reference/android/support/v7/graphics/Palette.html


## Install

[![Nuget](https://img.shields.io/nuget/v/PaletteNet)](https://www.nuget.org/packages/PaletteNet) [https://www.nuget.org/packages/PaletteNet](https://www.nuget.org/packages/PaletteNet)

Target frameworks:
- net6.0
- net6.0-android
- net6.0-windows10.0.19041.0
- netstandard2.0
- net48

## Example (WinUI3)
```c#
using IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

PaletteColors palette = PaletteColors.Generate(new BitmapDecoderHelper(decoder));
Color dominantColor = palette.DominantColor;
IEnumerable<Color> allColors = palette.GetAllColors();
```
or
```c#
IBitmapHelper bitmapHelper = new BitmapDecoderHelper(decoder);
PaletteBuilder paletteBuilder = new PaletteBuilder();
Palette palette = paletteBuilder.Generate(bitmapHelper);
int? rgbColor = palette.MutedColor;
int? rgbTextColor = palette.DominantSwatch.TitleTextColor;
```

![screenshot 1](https://github.com/tmk907/PaletteNet/blob/master/images/example1.png "Example 1")
![screenshot 2](https://github.com/tmk907/PaletteNet/blob/master/images/example2.png "Example 2")

## Sample app

Download app from Microsoft Store

<a href='https://www.microsoft.com/en-us/p/palettenet-sample-app/9MTQD4S7C86H?cid=badgegithub'>
<img width='240' height='96'  src='https://get.microsoft.com/images/en-us%20dark.svg' 
alt='English badge'/></a>

[Color palette icons created by Freepik - Flaticon](https://www.flaticon.com/free-icons/color-palette)