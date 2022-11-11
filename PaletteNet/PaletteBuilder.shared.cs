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

using System;
using System.Collections.Generic;

namespace PaletteNet
{
    /// <summary>
    /// Builder class for generating Palette instances.
    /// </summary>
    public sealed class PaletteBuilder
    {
        /**
         * The default filter.
         */
        static readonly IFilter DEFAULT_FILTER = new DefaultFilter();

        static readonly int DEFAULT_CALCULATE_NUMBER_COLORS = 16;

        private readonly List<Target> _targets = new List<Target>();

        private int _maxColors = DEFAULT_CALCULATE_NUMBER_COLORS;

        private readonly List<IFilter> _filters = new List<IFilter>();

        public PaletteBuilder()
        {
            _filters.Add(DEFAULT_FILTER);

            // Add the default targets
            _targets.Add(Target.LIGHT_VIBRANT);
            _targets.Add(Target.VIBRANT);
            _targets.Add(Target.DARK_VIBRANT);
            _targets.Add(Target.LIGHT_MUTED);
            _targets.Add(Target.MUTED);
            _targets.Add(Target.DARK_MUTED);
        }

        /// <summary>
        /// Generate and return the Palette synchronously.
        /// </summary>
        /// <returns></returns>
        public Palette Generate(IBitmapHelper bitmapHelper)
        {
            if (bitmapHelper == null)
            {
                throw new ArgumentNullException(nameof(bitmapHelper));
            }

            var pixels = bitmapHelper.ScaleDownAndGetPixels();
            ColorCutQuantizer quantizer = new ColorCutQuantizer(
                    pixels,
                    _maxColors,
                    (_filters.Count == 0) ? null : _filters.ToArray());

            var swatches = quantizer.GetQuantizedColors();

            Palette palette = new Palette(swatches, _targets);
            palette.Generate();
            return palette;
        }

        /// <summary>
        /// Set the maximum number of colors to use in the quantization step when using a
        /// bitmap as the source.
        /// Good values for depend on the source image type. For landscapes, good values are in
        /// the range 10-16. For images which are largely made up of people's faces then this
        /// value should be increased to ~24.
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public PaletteBuilder MaximumColorCount(int colors)
        {
            _maxColors = colors;
            return this;
        }
        
        /// <summary>
        /// Add a filter to be able to have fine grained control over which colors are
        /// allowed in the resulting palette.
        /// </summary>
        /// <param name="filter">filter filter to add.</param>
        /// <returns></returns>
        public PaletteBuilder AddFilter(IFilter? filter)
        {
            if (filter != null)
            {
                _filters.Add(filter);
            }
            return this;
        }

        /// <summary>
        /// Clear all added filters. This includes any default filters added automatically by Palette.
        /// </summary>
        /// <returns></returns>
        public PaletteBuilder ClearFilters()
        {
            _filters.Clear();
            return this;
        }

        /// <summary>
        /// Add a target profile to be generated in the palette.
        /// You can retrieve the result via Palette.GetSwatchForTarget(Target)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public PaletteBuilder AddTarget(Target target)
        {
            if (!_targets.Contains(target))
            {
                _targets.Add(target);
            }
            return this;
        }

        /// <summary>
        /// Clear all added targets. This includes any default targets added automatically by Palette
        /// </summary>
        /// <returns></returns>
        public PaletteBuilder ClearTargets()
        {
            if (_targets != null)
            {
                _targets.Clear();
            }
            return this;
        }

        private class DefaultFilter : IFilter
        {
            private const float BLACK_MAX_LIGHTNESS = 0.05f;
            private const float WHITE_MIN_LIGHTNESS = 0.95f;

            public bool IsAllowed(int rgb, float[] hsl)
            {
                return !IsWhite(hsl) && !IsBlack(hsl) && !IsNearRedILine(hsl);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hslColor"></param>
            /// <returns>true if the color represents a color which is close to black.</returns>
            private bool IsBlack(float[] hslColor)
            {
                return hslColor[2] <= BLACK_MAX_LIGHTNESS;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hslColor"></param>
            /// <returns>true if the color represents a color which is close to white.</returns>
            private bool IsWhite(float[] hslColor)
            {
                return hslColor[2] >= WHITE_MIN_LIGHTNESS;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hslColor"></param>
            /// <returns>true if the color lies close to the red side of the I line.</returns>
            private bool IsNearRedILine(float[] hslColor)
            {
                return hslColor[0] >= 10f && hslColor[0] <= 37f && hslColor[1] <= 0.82f;
            }
        }
    }
}
