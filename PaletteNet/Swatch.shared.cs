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

        private readonly int mRed, mGreen, mBlue;
        private readonly int mRgb;
        private readonly int mPopulation;

        private bool mGeneratedTextColors;
        private int mTitleTextColor;
        private int mBodyTextColor;

        private float[] mHsl;

        public Swatch(int color, int population)
        {
            mRed = ColorHelpers.Red(color);
            mGreen = ColorHelpers.Green(color);
            mBlue = ColorHelpers.Blue(color);
            mRgb = color;
            mPopulation = population;
        }

        public Swatch(float[] hsl, int population) : this(ColorHelpers.HSLToColor(hsl), population)
        {
            mHsl = hsl;
        }

        public Swatch(int red, int green, int blue, int population)
        {
            mRed = red;
            mGreen = green;
            mBlue = blue;
            mRgb = ColorHelpers.RGB(red, green, blue);
            mPopulation = population;
        }

        public int GetRgb()
        {
            return mRgb;
        }

        /// <summary>
        /// Return this swatch's HSL values.
        /// hsv[0] is Hue[0.. 360)
        /// hsv[1] is Saturation[0...1]
        /// hsv[2] is Lightness[0...1]
        /// </summary>
        /// <returns></returns>
        public float[] GetHsl()
        {
            if (mHsl == null)
            {
                mHsl = new float[3];
            }
            ColorHelpers.RGBToHSL(mRed, mGreen, mBlue, mHsl);
            return mHsl;
        }

        /// <summary>
        /// return the number of pixels represented by this swatch
        /// </summary>
        /// <returns></returns>
        public int GetPopulation()
        {
            return mPopulation;
        }

        /// <summary>
        /// Returns an appropriate color to use for any 'title' text which is displayed over this
        /// Swatch's color. This color is guaranteed to have sufficient contrast.
        /// </summary>
        /// <returns></returns>
        public int getTitleTextColor()
        {
            ensureTextColorsGenerated();
            return mTitleTextColor;
        }

        /// <summary>
        /// Returns an appropriate color to use for any 'body' text which is displayed over this
        /// Swatch's color. This color is guaranteed to have sufficient contrast.
        /// </summary>
        /// <returns></returns>
        public int getBodyTextColor()
        {
            ensureTextColorsGenerated();
            return mBodyTextColor;
        }

        private void ensureTextColorsGenerated()
        {
            if (!mGeneratedTextColors)
            {
                // First check white, as most colors will be dark
                int lightBodyAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.WHITE, mRgb, MIN_CONTRAST_BODY_TEXT);
                int lightTitleAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.WHITE, mRgb, MIN_CONTRAST_TITLE_TEXT);

                if (lightBodyAlpha != -1 && lightTitleAlpha != -1)
                {
                    // If we found valid light values, use them and return
                    mBodyTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightBodyAlpha);
                    mTitleTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightTitleAlpha);
                    mGeneratedTextColors = true;
                    return;
                }

                int darkBodyAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.BLACK, mRgb, MIN_CONTRAST_BODY_TEXT);
                int darkTitleAlpha = ColorHelpers.CalculateMinimumAlpha(
                        ColorHelpers.BLACK, mRgb, MIN_CONTRAST_TITLE_TEXT);

                if (darkBodyAlpha != -1 && darkBodyAlpha != -1)
                {
                    // If we found valid dark values, use them and return
                    mBodyTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkBodyAlpha);
                    mTitleTextColor = ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkTitleAlpha);
                    mGeneratedTextColors = true;
                    return;
                }

                // If we reach here then we can not find title and body values which use the same
                // lightness, we need to use mismatched values
                mBodyTextColor = lightBodyAlpha != -1
                        ? ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightBodyAlpha)
                        : ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkBodyAlpha);
                mTitleTextColor = lightTitleAlpha != -1
                        ? ColorHelpers.SetAlphaComponent(ColorHelpers.WHITE, lightTitleAlpha)
                        : ColorHelpers.SetAlphaComponent(ColorHelpers.BLACK, darkTitleAlpha);
                mGeneratedTextColors = true;
            }
        }
    }
}
