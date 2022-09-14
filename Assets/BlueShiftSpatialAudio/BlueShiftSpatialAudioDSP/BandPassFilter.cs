using System;

namespace AudioFXToolkitDSP
{
    /****************
     * Bandpass Class 
     * --------------
     * A biquad based Bandpass filter. This filters out/attenuates the high and low end surrounding a desired frequency. 
     */

    public class BandpassFilter : BiquadFilter
    {

        /// <summary>
        /// Sets the BandPassFilter parameters.
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

            float b0 = K / (K2Q + K + Q);
            float b1 = 0;
            float b2 = -K / (K2Q + K + Q);
            float a1 = (2 * Q * (K * K - 1)) / (K2Q + K + Q);
            float a2 = (K2Q - K + Q) / (K2Q + K + Q);

            SetCoefficents(b0, b1, b2, a1, a2);
        }
    }
}
