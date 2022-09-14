using System;

namespace AudioFXToolkitDSP
{

    /****************
     * Distortion Class 
     * --------------
     * Two simple distortion algorithms: Clip and Soft.
     */

    public class Distortion
    {
        /// <summary>
        /// This is a simple clip distortion algorithm. Anything above 1 or below -1 is clipped to those values.
        /// This creates a distotion harmonics in similar to that of a square wave.
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// The input sample.
        /// 
        /// <param name="gain"></param>
        /// The predistortion gain.
        /// 
        /// <returns> The float value after passing through the distortion. </returns>

        public float Clip(float sample, float gain)
        {
            return Math.Clamp(sample * gain, -1.0f, 1.0f);
        }

        /// <summary>
        /// This is a simple soft distortion algorithm. It does this by passing the input through the arctan algorithm. 
        /// This creates a distortion in a less agressive way than the simple clip distortion. 
        /// </summary>
        /// 
        /// <param name="sample"></param>
        /// The input sample.
        /// 
        /// <param name="gain"></param>
        /// The predistortion gain.
        /// 
        /// <returns> The float value after passing through the distortion. </returns>

        public float Soft(float sample, float gain)
        {
            return (float)((2 / Math.PI) * Math.Atan(sample * gain));
        }
    }
}