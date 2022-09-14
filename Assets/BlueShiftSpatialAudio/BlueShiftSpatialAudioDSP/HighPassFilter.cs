using System;

namespace AudioFXToolkitDSP
{

    /****************
     * Highpass Class 
     * --------------
     * A biquad based highpass filter. This filters out/attenuates low end signals. 
     */

    public class HighPassFilter : BiquadFilter
    {

        /// <summary>
        /// Sets the HighPassFilter parameters. 
        /// </summary>
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        /// 
        /// <param name="frequency"></param>
        /// The desired cutoff frequency. 
        /// 
        /// <param name="Q"></param>
        /// The filter Q.
        ///

        public void SetFilterParameters(int sample_rate, float frequency, float Q = 0.707f)
        {
            //intermediate
            float K = (float)Math.Tan(Math.PI * frequency / sample_rate);

            //boost coefficents
            float K2Q = (K * K * Q);

            float b0 = Q / (K2Q + K + Q);
            float b1 = (-2 * Q) / (K2Q + K + Q);
            float b2 = Q / (K2Q + K + Q);
            float a1 = (2 * Q * (K * K - 1)) / (K2Q + K + Q);
            float a2 = (K2Q - K + Q) / (K2Q + K + Q);

            SetCoefficents(b0, b1, b2, a1, a2);
        }
    }
}
