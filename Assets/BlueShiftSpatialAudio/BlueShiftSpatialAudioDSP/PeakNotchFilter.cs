using System;

namespace AudioFXToolkitDSP
{

    /****************
     * PeakNotchFilter Class 
     * --------------
     * A biquad based peaknotch filter. This boosts a configurable part of the spectrum. This can be used to create multiband eqs, etc. 
     */

    public class PeakNotchFilter : BiquadFilter
    {
        /// <summary>
        /// Sets the PeakNotch filter parameters.  
        /// </summary>
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        /// 
        /// <param name="frequency"></param>
        /// The desired cutoff frequency. 
        /// 
        /// <param name="dBgain"></param>
        /// The change in dB of the filter. 
        /// 
        /// <param name="Q"></param>
        /// The filter Q. 

        public void SetFilterParameters(int sample_rate, float frequency, float dBgain = 0f, float Q = 0.707f)
        {

            float K = (float)Math.Tan((Math.PI * frequency) / sample_rate);
            float V0 = (float)Math.Pow(10, dBgain / 20);

            if (V0 < 1)
                V0 = 1 / V0;

            //boost coefficents
            if (dBgain >= 0)
            {
                float b0 = (1 + ((V0 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);
                float b1 = (2 * (K * K - 1)) / (1 + ((1 / Q) * K) + K * K);
                float b2 = (1 - ((V0 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);
                float a0 = 1;
                float a1 = b1;
                float a2 = (1 - ((1 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);

                SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);

            }
            else
            {
                float b0 = (1 + ((1 / Q) * K) + K * K) / (1 + ((V0 / Q) * K) + K * K);
                float b1 = (2 * (K * K - 1)) / (1 + ((V0 / Q) * K) + K * K);
                float b2 = (1 - ((1 / Q) * K) + K * K) / (1 + ((V0 / Q) * K) + K * K);
                float a1 = b1;
                float a0 = 1;
                float a2 = (1 - ((V0 / Q) * K) + K * K) / (1 + ((V0 / Q) * K) + K * K);

                SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);

            }
        }
    }
}
