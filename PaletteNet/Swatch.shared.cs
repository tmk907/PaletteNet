/*
 * Copyright 2015 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace PaletteNet
{
    /// <summary>
    /// Represents a color swatch generated from an image's palette.
    /// </summary>
    public sealed class Swatch
    {
        static readonly float MIN_CONTRAST_TITLE_TEXT = 3.0f;
        static readonly float MIN_CONTRAST_BODY_TEXT = 4.5f;

        private readonly int _red, _green, _blue;
        private readonly int _rgb;
        private readonly int _population;

        private bool _generatedTextColors;
        private int _titleTextColor;
        private int _bodyTextColor;

        private float[] _hsl;

        public Swatch(int color, int population)
        {
            _red = ColorHelpers.Red(color);
            _green = ColorHelpers.Green(color);
            _blue = ColorHelpers.Blue(color);
            _rgb = color;
            _population = population;
            _hsl = new float[3];
        }

        public Swatch(float[] hsl, int population) : this(ColorHelpers.HSLToColor(hsl), population)
        {
            _hsl = hsl;
        }

        public Swatch(int red, int green, int blue, int population)
        {
            _red = red;
            _green = green;
            _blue = blue;
            _rgb = ColorHelpers.RGB(red, green, blue);
            _population = population;
            _hsl = new float[3];
        }

        public int Rgb => _rgb;

        /// <summary>
        /// Return this swatch's HSL values.
        /// hsv[0] is Hue[0.. 360)
        /// hsv[1] is Saturation[0...1]
        /// hsv[2] is Lightness[0...1]
        /// </summary>
        /// <returns></returns>
        public float[] GetHsl()
        {
            if (_hsl == null)
            {
                _hsl = new float[3];
            }
            ColorHelpers.RGBToHSL(_red, _green, _blue, _hsl);
            return _hsl;
        }

        /// <summary>
        /// return the number of pixels represented by this swatch
        /// </summary>
        /// <returns></returns>
        public int Population => _population;

        /// <summary>
        /// Returns an appropriate color to use for any 'title' text which is displayed over this
        /// Swatch's color. This color is guaranteed to have sufficient contrast.
        /// </summary>
        /// <returns></returns>
        public int TitleTextColor
        {
            get
            {
                EnsureTextColorsGenerated();
                return _titleTextColor;
            }
        }

        /// <summary>
        /// Returns an appropriate color to use for any 'body' text which is displayed over this
        /// Swatch's color. This color is guaranteed to have sufficient contrast.
        /// </summary>
        /// <returns></returns>
        public int BodyTextColor
        {
            get
            {
                EnsureTextColorsGenerated();
                return _bodyTextColor;
            }
        }

        private void EnsureTextColorsGenerated()
        {
            if (!_generatedTextColors)
            {
                // First check white, as most colors will be dark
                int lightBodyAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.WHITE, _rgb, MIN_CONTRAST_BODY_TEXT);
                int lightTitleAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.WHITE, _rgb, MIN_CONTRAST_TITLE_TEXT);

                if (lightBodyAlpha != -1 && lightTitleAlpha != -1)
                {
                    // If we found valid light values, use them and return
                    _bodyTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightBodyAlpha);
                    _titleTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightTitleAlpha);
                    _generatedTextColors = true;
                    return;
                }

                int darkBodyAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.BLACK, _rgb, MIN_CONTRAST_BODY_TEXT);
                int darkTitleAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.BLACK, _rgb, MIN_CONTRAST_TITLE_TEXT);

                if (darkBodyAlpha != -1 && darkBodyAlpha != -1)
                {
                    // If we found valid dark values, use them and return
                    _bodyTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkBodyAlpha);
                    _titleTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkTitleAlpha);
                    _generatedTextColors = true;
                    return;
                }

                // If we reach here then we can not find title and body values which use the same
                // lightness, we need to use mismatched values
                _bodyTextColor = lightBodyAlpha != -1
                        ? ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightBodyAlpha)
                        : ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkBodyAlpha);
                _titleTextColor = lightTitleAlpha != -1
                        ? ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightTitleAlpha)
                        : ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkTitleAlpha);
                _generatedTextColors = true;
            }
        }
    }
}
