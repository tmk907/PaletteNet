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
    public class Palette
    {
        private readonly List<Swatch> mSwatches;
        private readonly List<Target> mTargets;

        private readonly Dictionary<Target, Swatch> mSelectedSwatches;
        private readonly Dictionary<int, bool> mUsedColors;

        private readonly Swatch mDominantSwatch;

        /// <summary>
        /// White
        /// </summary>
        const int DefaultColorValue = -1;
       
        public Palette(List<Swatch> swatches, List<Target> targets)
        {
            mSwatches = swatches;
            mTargets = targets;

            mUsedColors = new Dictionary<int, bool>();
            mSelectedSwatches = new Dictionary<Target, Swatch>();

            mDominantSwatch = FindDominantSwatch();
        }

        public void Generate()
        {
            // We need to make sure that the scored targets are generated first. This is so that
            // inherited targets have something to inherit from
            for (int i = 0, count = mTargets.Count; i < count; i++)
            {
                Target target = mTargets[i];
                target.NormalizeWeights();
                mSelectedSwatches.Add(target, GenerateScoredTarget(target));
            }
            // We now clear out the used colors
            mUsedColors.Clear();
        }

        #region Getters

        /// <summary>
        /// Returns all of the swatches which make up the palette.
        /// </summary>
        /// <returns></returns>
        public List<Swatch> GetSwatches()
        {
            return new List<Swatch>(mSwatches);
        }

        /// <summary>
        /// Returns the targets used to generate this palette.
        /// </summary>
        /// <returns></returns>
        public List<Target> GetTargets()
        {
            return new List<Target>(mTargets);
        }

        /// <summary>
        /// Returns the most vibrant swatch in the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch GetVibrantSwatch()
        {
            return GetSwatchForTarget(Target.VIBRANT);
        }

        /// <summary>
        /// Returns a light and vibrant swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch GetLightVibrantSwatch()
        {
            return GetSwatchForTarget(Target.LIGHT_VIBRANT);
        }

        /// <summary>
        /// Returns a dark and vibrant swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch GetDarkVibrantSwatch()
        {
            return GetSwatchForTarget(Target.DARK_VIBRANT);
        }

        /// <summary>
        /// Returns a muted swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch GetMutedSwatch()
        {
            return GetSwatchForTarget(Target.MUTED);
        }

        /// <summary>
        /// Returns a muted and light swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch GetLightMutedSwatch()
        {
            return GetSwatchForTarget(Target.LIGHT_MUTED);
        }

        /// <summary>
        /// Returns a muted and dark swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch GetDarkMutedSwatch()
        {
            return GetSwatchForTarget(Target.DARK_MUTED);
        }

        /// <summary>
        /// Returns the most vibrant color in the palette as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetVibrantColorValue(int defaultColor = DefaultColorValue)
        {
            return GetColorForTarget(Target.VIBRANT, defaultColor);
        }

        /// <summary>
        /// Returns a light and vibrant color from the palette as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetLightVibrantColorValue(int defaultColor = DefaultColorValue)
        {
            return GetColorForTarget(Target.LIGHT_VIBRANT, defaultColor);
        }

        /// <summary>
        /// Returns a dark and vibrant color from the palette as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetDarkVibrantColorValue(int defaultColor = DefaultColorValue)
        {
            return GetColorForTarget(Target.DARK_VIBRANT, defaultColor);
        }

        /// <summary>
        /// Returns a muted color from the palette as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetMutedColorValue(int defaultColor = DefaultColorValue)
        {
            return GetColorForTarget(Target.MUTED, defaultColor);
        }

        /// <summary>
        /// Returns a muted and light color from the palette as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetLightMutedColorValue(int defaultColor = DefaultColorValue)
        {
            return GetColorForTarget(Target.LIGHT_MUTED, defaultColor);
        }

        /// <summary>
        /// Returns a muted and dark color from the palette as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetDarkMutedColorValue(int defaultColor = DefaultColorValue)
        {
            return GetColorForTarget(Target.DARK_MUTED, defaultColor);
        }

        /// <summary>
        /// Returns the selected swatch for the given target from the palette, or null if one
        /// could not be found.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Swatch GetSwatchForTarget(Target target)
        {
            return mSelectedSwatches[target];
        }

        /// <summary>
        /// Returns the selected color for the given target from the palette as an RGB packed int.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="defaultColor"></param>
        /// <returns></returns>
        public int GetColorForTarget(Target target, int defaultColor = DefaultColorValue)
        {
            Swatch swatch = GetSwatchForTarget(target);
            return swatch != null ? swatch.GetRgb() : defaultColor;
        }

        /// <summary>
        /// Returns the dominant swatch from the palette.
        /// The dominant swatch is defined as the swatch with the greatest population(frequency)
        /// within the palette.
        /// </summary>
        /// <returns></returns>
        public Swatch GetDominantSwatch()
        {
            return mDominantSwatch;
        }

        /// <summary>
        /// Returns the color of the dominant swatch from the palette, as an RGB packed int.
        /// </summary>
        /// <param name="defaultColor">value to return if the swatch isn't available</param>
        /// <returns></returns>
        public int GetDominantColorValue(int defaultColor = DefaultColorValue)
        {
            return mDominantSwatch != null ? mDominantSwatch.GetRgb() : defaultColor;
        }
        #endregion

        private Swatch GenerateScoredTarget(Target target)
        {
            Swatch maxScoreSwatch = GetMaxScoredSwatchForTarget(target);
            if (maxScoreSwatch != null && target.IsExclusive())
            {
                // If we have a swatch, and the target is exclusive, add the color to the used list
                mUsedColors.Add(maxScoreSwatch.GetRgb(), true);
            }
            return maxScoreSwatch;
        }

        private Swatch GetMaxScoredSwatchForTarget(Target target)
        {
            float maxScore = 0;
            Swatch maxScoreSwatch = null;
            for (int i = 0, count = mSwatches.Count; i < count; i++)
            {
                Swatch swatch = mSwatches[i];
                if (ShouldBeScoredForTarget(swatch, target))
                {
                    float score = GenerateScore(swatch, target);
                    if (maxScoreSwatch == null || score > maxScore)
                    {
                        maxScoreSwatch = swatch;
                        maxScore = score;
                    }
                }
            }
            return maxScoreSwatch;
        }

        private bool ShouldBeScoredForTarget(Swatch swatch, Target target)
        {
            // Check whether the HSL values are within the correct ranges, and this color hasn't
            // been used yet.
            float[] hsl = swatch.GetHsl();
            bool a = false;
            mUsedColors.TryGetValue(swatch.GetRgb(), out a);
            return hsl[1] >= target.GetMinimumSaturation() && hsl[1] <= target.GetMaximumSaturation()
                    && hsl[2] >= target.GetMinimumLightness() && hsl[2] <= target.GetMaximumLightness()
                    && !a;
        }

        private float GenerateScore(Swatch swatch, Target target)
        {
            float[] hsl = swatch.GetHsl();

            float saturationScore = 0;
            float luminanceScore = 0;
            float populationScore = 0;

            int maxPopulation = mDominantSwatch != null ? mDominantSwatch.GetPopulation() : 1;

            if (target.GetSaturationWeight() > 0)
            {
                saturationScore = target.GetSaturationWeight()
                        * (1f - Math.Abs(hsl[1] - target.GetTargetSaturation()));
            }
            if (target.GetLightnessWeight() > 0)
            {
                luminanceScore = target.GetLightnessWeight()
                        * (1f - Math.Abs(hsl[2] - target.GetTargetLightness()));
            }
            if (target.GetPopulationWeight() > 0)
            {
                populationScore = target.GetPopulationWeight()
                        * (swatch.GetPopulation() / (float)maxPopulation);
            }

            return saturationScore + luminanceScore + populationScore;
        }

        private Swatch FindDominantSwatch()
        {
            int maxPop = Int32.MinValue;
            Swatch maxSwatch = null;
            for (int i = 0, count = mSwatches.Count; i < count; i++)
            {
                Swatch swatch = mSwatches[i];
                if (swatch.GetPopulation() > maxPop)
                {
                    maxSwatch = swatch;
                    maxPop = swatch.GetPopulation();
                }
            }
            return maxSwatch;
        }  
    }
}
