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
    /// A Filter provides a mechanism for exercising fine-grained control over which colors
    /// are valid within a resulting Palette.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Hook to allow clients to be able filter colors from resulting palette.
        /// </summary>
        /// <param name="rgb">rgb the color in RGB888.</param>
        /// <param name="hsl">HSL representation of the color.</param>
        /// <returns>true if the color is allowed, false if not.</returns>
        bool IsAllowed(int rgb, float[] hsl);
    }
}
