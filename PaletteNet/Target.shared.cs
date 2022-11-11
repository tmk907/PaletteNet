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

namespace PaletteNet
{
    public class Target
    {
        private const float TARGET_DARK_LUMA = 0.26f;
        private const float MAX_DARK_LUMA = 0.45f;

        private const float MIN_LIGHT_LUMA = 0.55f;
        private const float TARGET_LIGHT_LUMA = 0.74f;

        private const float MIN_NORMAL_LUMA = 0.3f;
        private const float TARGET_NORMAL_LUMA = 0.5f;
        private const float MAX_NORMAL_LUMA = 0.7f;

        private const float TARGET_MUTED_SATURATION = 0.3f;
        private const float MAX_MUTED_SATURATION = 0.4f;

        private const float TARGET_VIBRANT_SATURATION = 1f;
        private const float MIN_VIBRANT_SATURATION = 0.35f;

        private const float WEIGHT_SATURATION = 0.24f;
        private const float WEIGHT_LUMA = 0.52f;
        private const float WEIGHT_POPULATION = 0.24f;

        const int INDEX_MIN = 0;
        const int INDEX_TARGET = 1;
        const int INDEX_MAX = 2;

        const int INDEX_WEIGHT_SAT = 0;
        const int INDEX_WEIGHT_LUMA = 1;
        const int INDEX_WEIGHT_POP = 2;

        /// <summary>
        /// A target which has the characteristics of a vibrant color which is light in luminance.
        /// </summary>
        public static readonly Target LIGHT_VIBRANT;

        /// <summary>
        /// A target which has the characteristics of a vibrant color which is neither light or dark.
        /// </summary>
        public static readonly Target VIBRANT;

        /// <summary>
        /// A target which has the characteristics of a vibrant color which is dark in luminance.
        /// </summary>
        public static readonly Target DARK_VIBRANT;

        /// <summary>
        /// A target which has the characteristics of a muted color which is light in luminance.
        /// </summary>
        public static readonly Target LIGHT_MUTED;

        /// <summary>
        /// A target which has the characteristics of a muted color which is neither light or dark.
        /// </summary>
        public static readonly Target MUTED;

        /// <summary>
        /// A target which has the characteristics of a muted color which is dark in luminance.
        /// </summary>
        public static readonly Target DARK_MUTED;

        static Target()
        {
            LIGHT_VIBRANT = new Target();
            SetDefaultLightLightnessValues(LIGHT_VIBRANT);
            SetDefaultVibrantSaturationValues(LIGHT_VIBRANT);

            VIBRANT = new Target();
            SetDefaultNormalLightnessValues(VIBRANT);
            SetDefaultVibrantSaturationValues(VIBRANT);

            DARK_VIBRANT = new Target();
            SetDefaultDarkLightnessValues(DARK_VIBRANT);
            SetDefaultVibrantSaturationValues(DARK_VIBRANT);

            LIGHT_MUTED = new Target();
            SetDefaultLightLightnessValues(LIGHT_MUTED);
            SetDefaultMutedSaturationValues(LIGHT_MUTED);

            MUTED = new Target();
            SetDefaultNormalLightnessValues(MUTED);
            SetDefaultMutedSaturationValues(MUTED);

            DARK_MUTED = new Target();
            SetDefaultDarkLightnessValues(DARK_MUTED);
            SetDefaultMutedSaturationValues(DARK_MUTED);
        }

        readonly float[] _saturationTargets = new float[3];
        readonly float[] _lightnessTargets = new float[3];
        readonly float[] _weights = new float[3];
        bool _isExclusive = true; // default to true

        public Target()
        {
            SetTargetDefaultValues(_saturationTargets);
            SetTargetDefaultValues(_lightnessTargets);
            SetDefaultWeights();
        }

        public Target(Target from)
        {
            Array.Copy(from._saturationTargets, 0, _saturationTargets, 0,
                    _saturationTargets.Length);
            Array.Copy(from._lightnessTargets, 0, _lightnessTargets, 0,
                    _lightnessTargets.Length);
            Array.Copy(from._weights, 0, _weights, 0, _weights.Length);
        }

        #region Getters

        /// <summary>
        /// The minimum saturation value for this target.
        /// </summary>
        /// <returns>FloatRange(from = 0, to = 1)</returns>
        public float MinimumSaturation => _saturationTargets[INDEX_MIN];

        /// <summary>
        /// The target saturation value for this target.
        /// </summary>
        /// <returns>FloatRange(from = 0, to = 1)</returns>
        public float TargetSaturation => _saturationTargets[INDEX_TARGET];

        /// <summary>
        /// The maximum saturation value for this target.
        /// </summary>
        /// <returns>FloatRange(from = 0, to = 1)</returns>
        public float MaximumSaturation => _saturationTargets[INDEX_MAX];

        /// <summary>
        /// The minimum lightness value for this target.
        /// </summary>
        /// <returns>FloatRange(from = 0, to = 1)</returns>
        public float MinimumLightness => _lightnessTargets[INDEX_MIN];

        /// <summary>
        /// The target lightness value for this target.
        /// </summary>
        /// <returns>FloatRange(from = 0, to = 1)</returns>
        public float TargetLightness => _lightnessTargets[INDEX_TARGET];

        /// <summary>
        /// The maximum lightness value for this target.
        /// </summary>
        /// <returns>FloatRange(from = 0, to = 1)</returns>
        public float MaximumLightness => _lightnessTargets[INDEX_MAX];

        /// <summary>
        /// Returns the weight of importance that this target places on a color's saturation within
        /// the image.
        /// The larger the weight, relative to the other weights, the more important that a color
        /// being close to the target value has on selection.
        /// </summary>
        /// <returns></returns>
        public float SaturationWeight => _weights[INDEX_WEIGHT_SAT];

        /// <summary>
        /// Returns the weight of importance that this target places on a color's lightness within
        /// the image.
        /// The larger the weight, relative to the other weights, the more important that a color
        /// being close to the target value has on selection.
        /// </summary>
        /// <returns></returns>
        public float LightnessWeight => _weights[INDEX_WEIGHT_LUMA];

        /// <summary>
        /// Returns the weight of importance that this target places on a color's population within
        /// the image.
        /// The larger the weight, relative to the other weights, the more important that a
        /// color's population being close to the most populous has on selection.
        /// </summary>
        /// <returns></returns>
        public float PopulationWeight => _weights[INDEX_WEIGHT_POP];

        #endregion

        /// <summary>
        /// Returns whether any color selected for this target is exclusive for this target only.
        /// If false, then the color can be selected for other targets.
        /// </summary>
        /// <returns></returns>
        public bool IsExclusive()
        {
            return _isExclusive;
        }

        private static void SetTargetDefaultValues(float[] values)
        {
            values[INDEX_MIN] = 0f;
            values[INDEX_TARGET] = 0.5f;
            values[INDEX_MAX] = 1f;
        }

        private void SetDefaultWeights()
        {
            _weights[INDEX_WEIGHT_SAT] = WEIGHT_SATURATION;
            _weights[INDEX_WEIGHT_LUMA] = WEIGHT_LUMA;
            _weights[INDEX_WEIGHT_POP] = WEIGHT_POPULATION;
        }

        public void NormalizeWeights()
        {
            float sum = 0;
            for (int i = 0, z = _weights.Length; i < z; i++)
            {
                float weight = _weights[i];
                if (weight > 0)
                {
                    sum += weight;
                }
            }
            if (sum != 0)
            {
                for (int i = 0, z = _weights.Length; i < z; i++)
                {
                    if (_weights[i] > 0)
                    {
                        _weights[i] /= sum;
                    }
                }
            }
        }

        private static void SetDefaultDarkLightnessValues(Target target)
        {
            target._lightnessTargets[INDEX_TARGET] = TARGET_DARK_LUMA;
            target._lightnessTargets[INDEX_MAX] = MAX_DARK_LUMA;
        }

        private static void SetDefaultNormalLightnessValues(Target target)
        {
            target._lightnessTargets[INDEX_MIN] = MIN_NORMAL_LUMA;
            target._lightnessTargets[INDEX_TARGET] = TARGET_NORMAL_LUMA;
            target._lightnessTargets[INDEX_MAX] = MAX_NORMAL_LUMA;
        }

        private static void SetDefaultLightLightnessValues(Target target)
        {
            target._lightnessTargets[INDEX_MIN] = MIN_LIGHT_LUMA;
            target._lightnessTargets[INDEX_TARGET] = TARGET_LIGHT_LUMA;
        }

        private static void SetDefaultVibrantSaturationValues(Target target)
        {
            target._saturationTargets[INDEX_MIN] = MIN_VIBRANT_SATURATION;
            target._saturationTargets[INDEX_TARGET] = TARGET_VIBRANT_SATURATION;
        }

        private static void SetDefaultMutedSaturationValues(Target target)
        {
            target._saturationTargets[INDEX_TARGET] = TARGET_MUTED_SATURATION;
            target._saturationTargets[INDEX_MAX] = MAX_MUTED_SATURATION;
        }


        /// <summary>
        /// Builder class for generating custom Target instances.
        /// </summary>
        public sealed class Builder
        {
            private readonly Target _target;

            /// <summary>
            /// Create a new Target builder from scratch.
            /// </summary>
            public Builder()
            {
                _target = new Target();
            }

            /// <summary>
            /// Create a new builder based on an existing Target
            /// </summary>
            /// <param name="target"></param>
            public Builder(Target target)
            {
                _target = new Target(target);
            }

            /// <summary>
            /// Set the minimum saturation value for this target.
            /// </summary>
            /// <param name="value">FloatRange(from = 0, to = 1)</param>
            /// <returns></returns>
            public Builder SetMinimumSaturation(float value)
            {
                _target._saturationTargets[INDEX_MIN] = value;
                return this;
            }

            /// <summary>
            /// Set the target/ideal saturation value for this target.
            /// </summary>
            /// <param name="value">FloatRange(from = 0, to = 1)</param>
            /// <returns></returns>
            public Builder SetTargetSaturation(float value)
            {
                _target._saturationTargets[INDEX_TARGET] = value;
                return this;
            }

            /// <summary>
            /// Set the maximum saturation value for this target.
            /// </summary>
            /// <param name="value">FloatRange(from = 0, to = 1)</param>
            /// <returns></returns>
            public Builder SetMaximumSaturation(float value)
            {
                _target._saturationTargets[INDEX_MAX] = value;
                return this;
            }

            /// <summary>
            /// Set the minimum lightness value for this target.
            /// </summary>
            /// <param name="value">FloatRange(from = 0, to = 1)</param>
            /// <returns></returns>
            public Builder SetMinimumLightness(float value)
            {
                _target._lightnessTargets[INDEX_MIN] = value;
                return this;
            }

            /// <summary>
            /// Set the target/ideal lightness value for this target.
            /// </summary>
            /// <param name="value">FloatRange(from = 0, to = 1)</param>
            /// <returns></returns>
            public Builder SetTargetLightness(float value)
            {
                _target._lightnessTargets[INDEX_TARGET] = value;
                return this;
            }

            /// <summary>
            /// Set the maximum lightness value for this target.
            /// </summary>
            /// <param name="value">FloatRange(from = 0, to = 1)</param>
            /// <returns></returns>
            public Builder SetMaximumLightness(float value)
            {
                _target._lightnessTargets[INDEX_MAX] = value;
                return this;
            }

            /// <summary>
            /// Set the weight of importance that this target will place on saturation values.
            /// The larger the weight, relative to the other weights, the more important that a color
            /// being close to the target value has on selection.
            /// A weight of 0 means that it has no weight, and thus has no
            /// bearing on the selection.
            /// </summary>
            /// <param name="weight">FloatRange(from = 0)</param>
            /// <returns></returns>
            public Builder SetSaturationWeight(float weight)
            {
                _target._weights[INDEX_WEIGHT_SAT] = weight;
                return this;
            }

            /// <summary>
            /// Set the weight of importance that this target will place on lightness values.
            /// The larger the weight, relative to the other weights, the more important that a color
            /// being close to the target value has on selection.
            /// A weight of 0 means that it has no weight, and thus has no
            /// bearing on the selection.
            /// </summary>
            /// <param name="weight">FloatRange(from = 0)</param>
            /// <returns></returns>
            public Builder SetLightnessWeight(float weight)
            {
                _target._weights[INDEX_WEIGHT_LUMA] = weight;
                return this;
            }

            /// <summary>
            /// Set the weight of importance that this target will place on a color's population within
            /// the image.
            /// The larger the weight, relative to the other weights, the more important that a
            /// color's population being close to the most populous has on selection.
            /// A weight of 0 means that it has no weight, and thus has no
            /// bearing on the selection.
            /// </summary>
            /// <param name="weight">FloatRange(from = 0)</param>
            /// <returns></returns>
            public Builder SetPopulationWeight(float weight)
            {
                _target._weights[INDEX_WEIGHT_POP] = weight;
                return this;
            }

            /// <summary>
            /// Set whether any color selected for this target is exclusive to this target only.
            /// Defaults to true.
            /// </summary>
            /// <param name="exclusive">exclusive true if any the color is exclusive to this target, or false is the color can be selected for other targets.</param>
            /// <returns></returns>
            public Builder SetExclusive(bool exclusive)
            {
                _target._isExclusive = exclusive;
                return this;
            }

            /// <summary>
            /// Builds and returns the resulting Target
            /// </summary>
            /// <returns></returns>
            public Target Build()
            {
                return _target;
            }
        }
    }
}
