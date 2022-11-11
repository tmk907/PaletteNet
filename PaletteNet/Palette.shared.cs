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
        private readonly List<Swatch> _swatches;
        private readonly List<Target> _targets;

        private readonly Dictionary<Target, Swatch?> _SelectedSwatches;
        private readonly Dictionary<int, bool> _usedColors;

        private readonly Swatch? _dominantSwatch;

        public Palette(List<Swatch> swatches, List<Target> targets)
        {
            _swatches = swatches;
            _targets = targets;

            _usedColors = new Dictionary<int, bool>();
            _SelectedSwatches = new Dictionary<Target, Swatch?>();

            _dominantSwatch = FindDominantSwatch();
        }

        public void Generate()
        {
            // We need to make sure that the scored targets are generated first. This is so that
            // inherited targets have something to inherit from
            for (int i = 0, count = _targets.Count; i < count; i++)
            {
                Target target = _targets[i];
                target.NormalizeWeights();
                _SelectedSwatches.Add(target, GenerateScoredTarget(target));
            }
            // We now clear out the used colors
            _usedColors.Clear();
        }

        #region Getters

        /// <summary>
        /// Returns all of the swatches which make up the palette.
        /// </summary>
        /// <returns></returns>
        public List<Swatch> Swatches => new List<Swatch>(_swatches);

        /// <summary>
        /// Returns the targets used to generate this palette.
        /// </summary>
        /// <returns></returns>
        public List<Target> Targets => new List<Target>(_targets);

        /// <summary>
        /// Returns the most vibrant swatch in the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch? VibrantSwatch => GetSwatchForTarget(Target.VIBRANT);

        /// <summary>
        /// Returns a light and vibrant swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch? LightVibrantSwatch => GetSwatchForTarget(Target.LIGHT_VIBRANT);

        /// <summary>
        /// Returns a dark and vibrant swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch? DarkVibrantSwatch => GetSwatchForTarget(Target.DARK_VIBRANT);

        /// <summary>
        /// Returns a muted swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch? MutedSwatch => GetSwatchForTarget(Target.MUTED);

        /// <summary>
        /// Returns a muted and light swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch? LightMutedSwatch => GetSwatchForTarget(Target.LIGHT_MUTED);

        /// <summary>
        /// Returns a muted and dark swatch from the palette. Might be null.
        /// </summary>
        /// <returns></returns>
        public Swatch? DarkMutedSwatch => GetSwatchForTarget(Target.DARK_MUTED);

        /// <summary>
        /// Returns the most vibrant color in the palette as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? VibrantColor => GetColorForTarget(Target.VIBRANT);

        /// <summary>
        /// Returns a light and vibrant color from the palette as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? LightVibrantColor => GetColorForTarget(Target.LIGHT_VIBRANT);

        /// <summary>
        /// Returns a dark and vibrant color from the palette as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? DarkVibrantColor => GetColorForTarget(Target.DARK_VIBRANT);

        /// <summary>
        /// Returns a muted color from the palette as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? MutedColor => GetColorForTarget(Target.MUTED);

        /// <summary>
        /// Returns a muted and light color from the palette as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? LightMutedColor => GetColorForTarget(Target.LIGHT_MUTED);

        /// <summary>
        /// Returns a muted and dark color from the palette as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? DarkMutedColor => GetColorForTarget(Target.DARK_MUTED);

        /// <summary>
        /// Returns the selected swatch for the given target from the palette, or null if one
        /// could not be found.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Swatch? GetSwatchForTarget(Target target)
        {
            return _SelectedSwatches[target];
        }

        /// <summary>
        /// Returns the selected color for the given target from the palette as an RGB packed int.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int? GetColorForTarget(Target target)
        {
            var swatch = GetSwatchForTarget(target);
            return swatch?.Rgb;
        }

        /// <summary>
        /// Returns the dominant swatch from the palette.
        /// The dominant swatch is defined as the swatch with the greatest population(frequency)
        /// within the palette.
        /// </summary>
        /// <returns></returns>
        public Swatch? DominantSwatch => _dominantSwatch;

        /// <summary>
        /// Returns the color of the dominant swatch from the palette, as an RGB packed int.
        /// </summary>
        /// <returns></returns>
        public int? DominantColor => _dominantSwatch?.Rgb;
        #endregion

        private Swatch? GenerateScoredTarget(Target target)
        {
            Swatch? maxScoreSwatch = GetMaxScoredSwatchForTarget(target);
            if (maxScoreSwatch != null && target.IsExclusive())
            {
                // If we have a swatch, and the target is exclusive, add the color to the used list
                _usedColors.Add(maxScoreSwatch.Rgb, true);
            }
            return maxScoreSwatch;
        }

        private Swatch? GetMaxScoredSwatchForTarget(Target target)
        {
            float maxScore = 0;
            Swatch? maxScoreSwatch = null;
            for (int i = 0, count = _swatches.Count; i < count; i++)
            {
                Swatch swatch = _swatches[i];
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
            _usedColors.TryGetValue(swatch.Rgb, out a);
            return hsl[1] >= target.MinimumSaturation && hsl[1] <= target.MaximumSaturation
                    && hsl[2] >= target.MinimumLightness && hsl[2] <= target.MaximumLightness
                    && !a;
        }

        private float GenerateScore(Swatch swatch, Target target)
        {
            float[] hsl = swatch.GetHsl();

            float saturationScore = 0;
            float luminanceScore = 0;
            float populationScore = 0;

            int maxPopulation = _dominantSwatch != null ? _dominantSwatch.Population : 1;

            if (target.SaturationWeight > 0)
            {
                saturationScore = target.SaturationWeight
                        * (1f - Math.Abs(hsl[1] - target.TargetSaturation));
            }
            if (target.LightnessWeight > 0)
            {
                luminanceScore = target.LightnessWeight
                        * (1f - Math.Abs(hsl[2] - target.TargetLightness));
            }
            if (target.PopulationWeight > 0)
            {
                populationScore = target.PopulationWeight
                        * (swatch.Population / (float)maxPopulation);
            }

            return saturationScore + luminanceScore + populationScore;
        }

        private Swatch? FindDominantSwatch()
        {
            int maxPop = int.MinValue;
            Swatch? maxSwatch = null;
            for (int i = 0, count = _swatches.Count; i < count; i++)
            {
                Swatch swatch = _swatches[i];
                if (swatch.Population > maxPop)
                {
                    maxSwatch = swatch;
                    maxPop = swatch.Population;
                }
            }
            return maxSwatch;
        }  
    }
}
