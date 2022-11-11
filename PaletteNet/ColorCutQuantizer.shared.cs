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
    /**
     * An color quantizer based on the Median-cut algorithm, but optimized for picking out distinct
     * colors rather than representation colors.
     *
     * The color space is represented as a 3-dimensional cube with each dimension being an RGB
     * component. The cube is then repeatedly divided until we have reduced the color space to the
     * requested number of colors. An average color is then generated from each cube.
     *
     * What makes this different to median-cut is that median-cut divided cubes so that all of the cubes
     * have roughly the same population, where this quantizer divides boxes based on their color volume.
     * This means that the color space is divided into distinct colors, rather than representative
     * colors.
     */
    public class ColorCutQuantizer
    {
        private const int COMPONENT_RED = -3;
        private const int COMPONENT_GREEN = -2;
        private const int COMPONENT_BLUE = -1;

        private const int QUANTIZE_WORD_WIDTH = 5;
        private const int QUANTIZE_WORD_MASK = (1 << QUANTIZE_WORD_WIDTH) - 1;

        private int[] _colors;
        private int[] _histogram;
        private readonly List<Swatch> _quantizedColors;
        private readonly IFilter[]? _filters;

        private readonly float[] _tempHsl = new float[3];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels">histogram representing an image's pixel data</param>
        /// <param name="maxColors">The maximum number of colors that should be in the result palette.</param>
        /// <param name="filters">Set of filters to use in the quantization stage</param>
        public ColorCutQuantizer(int[] pixels, int maxColors, IFilter[]? filters)
        {
            _filters = filters;

            _histogram = new int[1 << (QUANTIZE_WORD_WIDTH * 3)];
            int[] hist = _histogram;
            for (int i = 0; i < pixels.Length; i++)
            {
                int quantizedColor = QuantizeFromRgb888(pixels[i]);
                // Now update the pixel value to the quantized value
                pixels[i] = quantizedColor;
                // And update the histogram
                hist[quantizedColor]++;
            }

            // Now let's count the number of distinct colors
            int distinctColorCount = 0;
            for (int color = 0; color < hist.Length; color++)
            {
                if (hist[color] > 0 && ShouldIgnoreColor(color))
                {
                    // If we should ignore the color, set the population to 0
                    hist[color] = 0;
                }
                if (hist[color] > 0)
                {
                    // If the color has population, increase the distinct color count
                    distinctColorCount++;
                }
            }

            // Now lets go through create an array consisting of only distinct colors
            _colors = new int[distinctColorCount];
            int[] colors = _colors;
            int distinctColorIndex = 0;
            for (int color = 0; color < hist.Length; color++)
            {
                if (hist[color] > 0)
                {
                    colors[distinctColorIndex++] = color;
                }
            }

            if (distinctColorCount <= maxColors)
            {
                // The image has fewer colors than the maximum requested, so just return the colors
                _quantizedColors = new List<Swatch>();
                foreach (int color in colors)
                {
                    _quantizedColors.Add(new Swatch(ApproximateToRgb888(color), hist[color]));
                }
            }
            else
            {
                // We need use quantization to reduce the number of colors
                _quantizedColors = QuantizePixels(maxColors);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the list of quantized colors</returns>
        public List<Swatch> GetQuantizedColors()
        {
            return _quantizedColors;
        }

        private List<Swatch> QuantizePixels(int maxColors)
        {
            // Create the priority queue which is sorted by volume descending. This means we always
            // split the largest box in the queue
            MaxHeap<Vbox> pq = new MaxHeap<Vbox>(new VBOX_COMPARATOR_VOLUME());

            // To start, offer a box which contains all of the colors
            pq.Add(new Vbox(0, _colors.Length - 1, _colors, _histogram));

            // Now go through the boxes, splitting them until we have reached maxColors or there are no
            // more boxes to split
            SplitBoxes(pq, maxColors);

            // Finally, return the average colors of the color boxes
            return GenerateAverageColors(pq);
        }

        /// <summary>
        /// Iterate through the Queue, popping ColorCutQuantizer.Vbox objects from the queue and splitting them. 
        /// Once split, the new box and the remaining box are offered back to the queue.
        /// </summary>
        /// <param name="queue">PriorityQueue to poll for boxes</param>
        /// <param name="maxSize">Maximum amount of boxes to split</param>
        private void SplitBoxes(MaxHeap<Vbox> queue, int maxSize)
        {
            while (queue.Count < maxSize)
            {
                var vbox = queue.ExtractDominating();

                if (vbox != null && vbox.CanSplit())
                {
                    // First split the box, and offer the result
                    queue.Add(vbox.SplitBox(_colors, _histogram));

                    // Then offer the box back
                    queue.Add(vbox);
                }
                else
                {
                    // If we get here then there are no more boxes to split, so return
                    return;
                }
            }
        }

        private List<Swatch> GenerateAverageColors(Heap<Vbox> vboxes)
        {
            List<Swatch> colors = new List<Swatch>(vboxes.Count);
            foreach (Vbox vbox in vboxes)
            {
                Swatch swatch = vbox.GetAverageColor(_colors, _histogram);
                if (!ShouldIgnoreColor(swatch))
                {
                    // As we're averaging a color box, we can still get colors which we do not want, so
                    // we check again here
                    colors.Add(swatch);
                }
            }
            return colors;
        }

        /// <summary>
        /// Represents a tightly fitting box around a color space.
        /// </summary>
        private class Vbox
        {
            // lower and upper index are inclusive
            private readonly int _lowerIndex;
            private int _upperIndex;
            // Population of colors within this box
            private int _population;

            private int _minRed, _maxRed;
            private int _minGreen, _maxGreen;
            private int _minBlue, _maxBlue;

            public Vbox(int lowerIndex, int upperIndex, int[] colors, int[] histogram)
            {
                _lowerIndex = lowerIndex;
                _upperIndex = upperIndex;
                FitBox(colors, histogram);
            }

            public int GetVolume()
            {
                return (_maxRed - _minRed + 1) * (_maxGreen - _minGreen + 1) *
                        (_maxBlue - _minBlue + 1);
            }

            public bool CanSplit()
            {
                return GetColorCount() > 1;
            }

            public int GetColorCount()
            {
                return 1 + _upperIndex - _lowerIndex;
            }

            /// <summary>
            /// Recomputes the boundaries of this box to tightly fit the colors within the box.
            /// </summary>
            public void FitBox(int[] colors, int[] hist)
            {                
                // Reset the min and max to opposite values
                int minRed, minGreen, minBlue;
                minRed = minGreen = minBlue = Int32.MaxValue;
                int maxRed, maxGreen, maxBlue;
                maxRed = maxGreen = maxBlue = Int32.MinValue;
                int count = 0;

                for (int i = _lowerIndex; i <= _upperIndex; i++)
                {
                    int color = colors[i];
                    count += hist[color];

                    int r = QuantizedRed(color);
                    int g = QuantizedGreen(color);
                    int b = QuantizedBlue(color);
                    if (r > maxRed)
                    {
                        maxRed = r;
                    }
                    if (r < minRed)
                    {
                        minRed = r;
                    }
                    if (g > maxGreen)
                    {
                        maxGreen = g;
                    }
                    if (g < minGreen)
                    {
                        minGreen = g;
                    }
                    if (b > maxBlue)
                    {
                        maxBlue = b;
                    }
                    if (b < minBlue)
                    {
                        minBlue = b;
                    }
                }

                _minRed = minRed;
                _maxRed = maxRed;
                _minGreen = minGreen;
                _maxGreen = maxGreen;
                _minBlue = minBlue;
                _maxBlue = maxBlue;
                _population = count;
            }

            /// <summary>
            /// Split this color box at the mid-point along its longest dimension
            /// </summary>
            /// <returns>new ColorBox</returns>
            public Vbox SplitBox(int[] colors, int[] histogram)
            {
                if (!CanSplit())
                {
                    throw new Exception("Can not split a box with only 1 color");
                }

                // find median along the longest dimension
                int splitPoint = FindSplitPoint(colors, histogram);

                Vbox newBox = new Vbox(splitPoint + 1, _upperIndex, colors, histogram);

                // Now change this box's upperIndex and recompute the color boundaries
                _upperIndex = splitPoint;
                FitBox(colors, histogram);

                return newBox;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>dimension which this box is largest in</returns>
            int GetLongestColorDimension()
            {
                int redLength = _maxRed - _minRed;
                int greenLength = _maxGreen - _minGreen;
                int blueLength = _maxBlue - _minBlue;

                if (redLength >= greenLength && redLength >= blueLength)
                {
                    return COMPONENT_RED;
                }
                else if (greenLength >= redLength && greenLength >= blueLength)
                {
                    return COMPONENT_GREEN;
                }
                else
                {
                    return COMPONENT_BLUE;
                }
            }

            /// <summary>
            /// Finds the point within this box's lowerIndex and upperIndex index of where to split.
            /// This is calculated by finding the longest color dimension, and then sorting the
            /// sub-array based on that dimension value in each color.The colors are then iterated over
            /// until a color is found with at least the midpoint of the whole box's dimension midpoint.
            /// </summary>
            /// <returns>index of the colors array to split from</returns>
            int FindSplitPoint(int[] colors, int[] hist)
            {
                int longestDimension = GetLongestColorDimension();

                // We need to sort the colors in this box based on the longest color dimension.
                // As we can't use a Comparator to define the sort logic, we modify each color so that
                // its most significant is the desired dimension
                ModifySignificantOctet(colors, longestDimension, _lowerIndex, _upperIndex);

                // Now sort... Arrays.sort uses a exclusive toIndex so we need to add 1
                Array.Sort(colors, _lowerIndex, _upperIndex + 1 - _lowerIndex);

                // Now revert all of the colors so that they are packed as RGB again
                ModifySignificantOctet(colors, longestDimension, _lowerIndex, _upperIndex);

                int midPoint = _population / 2;
                for (int i = _lowerIndex, count = 0; i <= _upperIndex; i++)
                {
                    count += hist[colors[i]];
                    if (count >= midPoint)
                    {
                        // we never want to split on the upperIndex, as this will result in the same
                        // box
                        return Math.Min(_upperIndex - 1, i);
                    }
                }

                return _lowerIndex;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns>the average color of this box.</returns>
            public Swatch GetAverageColor(int[] colors, int[] hist)
            {
                int redSum = 0;
                int greenSum = 0;
                int blueSum = 0;
                int totalPopulation = 0;

                for (int i = _lowerIndex; i <= _upperIndex; i++)
                {
                    int color = colors[i];
                    int colorPopulation = hist[color];

                    totalPopulation += colorPopulation;
                    redSum += colorPopulation * QuantizedRed(color);
                    greenSum += colorPopulation * QuantizedGreen(color);
                    blueSum += colorPopulation * QuantizedBlue(color);
                }

                int redMean = (int)Math.Round(redSum / (float)totalPopulation);
                int greenMean = (int)Math.Round(greenSum / (float)totalPopulation);
                int blueMean = (int)Math.Round(blueSum / (float)totalPopulation);

                return new Swatch(ApproximateToRgb888(redMean, greenMean, blueMean), totalPopulation);
            }
        }

        /// <summary>
        /// Modify the significant octet in a packed color int. Allows sorting based on the value of a
        /// single color component.This relies on all components being the same word size.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="dimension"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        static void ModifySignificantOctet(int[] a, int dimension, int lower, int upper)
        {
            switch (dimension)
            {
                case COMPONENT_RED:
                    // Already in RGB, no need to do anything
                    break;
                case COMPONENT_GREEN:
                    // We need to do a RGB to GRB swap, or vice-versa
                    for (int i = lower; i <= upper; i++)
                    {
                        int color = a[i];
                        a[i] = QuantizedGreen(color) << (QUANTIZE_WORD_WIDTH + QUANTIZE_WORD_WIDTH)
                                | QuantizedRed(color) << QUANTIZE_WORD_WIDTH
                                | QuantizedBlue(color);
                    }
                    break;
                case COMPONENT_BLUE:
                    // We need to do a RGB to BGR swap, or vice-versa
                    for (int i = lower; i <= upper; i++)
                    {
                        int color = a[i];
                        a[i] = QuantizedBlue(color) << (QUANTIZE_WORD_WIDTH + QUANTIZE_WORD_WIDTH)
                                | QuantizedGreen(color) << QUANTIZE_WORD_WIDTH
                                | QuantizedRed(color);
                    }
                    break;
            }
        }

        private bool ShouldIgnoreColor(int color565)
        {
            int rgb = ApproximateToRgb888(color565);
            ColorHelpers.ColorToHSL(rgb, _tempHsl);
            return ShouldIgnoreColor(rgb, _tempHsl);
        }

        private bool ShouldIgnoreColor(Swatch color)
        {
            return ShouldIgnoreColor(color.Rgb, color.GetHsl());
        }

        private bool ShouldIgnoreColor(int rgb, float[] hsl)
        {
            if (_filters != null && _filters.Length > 0)
            {
                for (int i = 0, count = _filters.Length; i < count; i++)
                {
                    if (!_filters[i].IsAllowed(rgb, hsl))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Comparator which sorts Vbox instances based on their volume, in descending order
        /// </summary>
        private class VBOX_COMPARATOR_VOLUME : Comparer<Vbox>
        {
            public override int Compare(Vbox? rhs, Vbox? lhs)
            {
                if (rhs == null && lhs != null) return -1;
                if (rhs == null && lhs == null) return 0;
                if (rhs != null && lhs == null) return 1;

                return rhs!.GetVolume() - lhs!.GetVolume();
            }
        }

        /// <summary>
        /// Quantized a RGB888 value to have a word width of QUANTIZE_WORD_WIDTH.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static int QuantizeFromRgb888(int color)
        {
            int r = ModifyWordWidth(ColorHelpers.Red(color), 8, QUANTIZE_WORD_WIDTH);
            int g = ModifyWordWidth(ColorHelpers.Green(color), 8, QUANTIZE_WORD_WIDTH);
            int b = ModifyWordWidth(ColorHelpers.Blue(color), 8, QUANTIZE_WORD_WIDTH);
            return r << (QUANTIZE_WORD_WIDTH + QUANTIZE_WORD_WIDTH) | g << QUANTIZE_WORD_WIDTH | b;
        }

        /// <summary>
        /// Quantized RGB888 values to have a word width of QUANTIZE_WORD_WIDTH.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int ApproximateToRgb888(int r, int g, int b)
        {
            return ColorHelpers.RGB(ModifyWordWidth(r, QUANTIZE_WORD_WIDTH, 8),
                    ModifyWordWidth(g, QUANTIZE_WORD_WIDTH, 8),
                    ModifyWordWidth(b, QUANTIZE_WORD_WIDTH, 8));
        }

        private static int ApproximateToRgb888(int color)
        {
            return ApproximateToRgb888(QuantizedRed(color), QuantizedGreen(color), QuantizedBlue(color));
        }

        /// <summary>
        /// red component of the quantized color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static int QuantizedRed(int color)
        {
            return (color >> (QUANTIZE_WORD_WIDTH + QUANTIZE_WORD_WIDTH)) & QUANTIZE_WORD_MASK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns>green component of a quantized color</returns>
        private static int QuantizedGreen(int color)
        {
            return (color >> QUANTIZE_WORD_WIDTH) & QUANTIZE_WORD_MASK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns>blue component of a quantized color</returns>
        private static int QuantizedBlue(int color)
        {
            return color & QUANTIZE_WORD_MASK;
        }

        private static int ModifyWordWidth(int value, int currentWidth, int targetWidth)
        {
            int newValue;
            if (targetWidth > currentWidth)
            {
                // If we're approximating up in word width, we'll shift up
                newValue = value << (targetWidth - currentWidth);
            }
            else
            {
                // Else, we will just shift and keep the MSB
                newValue = value >> (currentWidth - targetWidth);
            }
            return newValue & ((1 << targetWidth) - 1);
        }
    }
}
