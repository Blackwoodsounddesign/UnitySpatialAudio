using System;

namespace AudioFXToolkitDSP
{

    /****************
     * Highshelf Class 
     * --------------
     * A biquad based highshelf filter. This boosts/attenuates the high end of the signal. 
     */

    public class HighShelfFilter : BiquadFilter
    {
        /// <summary>
        /// Sets the HighShelfFilter parameters. 
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
        ///

        public void SetFilterParameters(int sample_rate, float frequency, float dBgain = 0f, float Q = 0.707f)
        {
            //intermediate
            float A = (float)Math.Pow(10, (dBgain / 40));
            float w0 = (float)(2 * Math.PI * frequency / sample_rate);
            float alpha = (float)(Math.Sin(w0) * 0.5 * Math.Sqrt((A + 1 / A) * (1 / Q - 1) + 2));

            //coefficents from RBJ cookbook
            float b0 = (float)(A * (A + 1 + (A - 1) * Math.Cos(w0) + 2 * Math.Sqrt(A) * alpha));
            float b1 = (float)(-2 * A * ((A - 1) + (A + 1) * Math.Cos(w0)));
            float b2 = (float)(A * ((A + 1) + (A - 1) * Math.Cos(w0) - 2 * Math.Sqrt(A) * alpha));
            float a0 = (float)((A + 1) - (A - 1) * Math.Cos(w0) + 2 * Math.Sqrt(A) * alpha);
            float a1 = (float)(2 * ((A - 1) - ((A + 1) * Math.Cos(w0))));
            float a2 = (float)((A + 1) - (A - 1) * Math.Cos(w0) - 2 * Math.Sqrt(A) * alpha);

            SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);
        }
    }
}
