using System;

namespace AudioFXToolkitDSP
{

    /****************
     * OnePole Filter Class 
     * --------------
     *  This is a simple one pole filter, set up to attenuate the high end of the signal. Commonly this is referred to as a lowpass filter.
     *  It can be turned into a highpass filter by subracting the filtered audio signal from the original audio signal. <br>
     *  https://en.wikipedia.org/wiki/Low-pass_filter
     */

    public class SimpleFilter
    {

        private float onepoleOut = 0;
        private float a0 = 1.0f;
        private float onepolecoefficent = 0;

        /// <summary>
        /// This sets the frequency and sample rate of the onepole filter.
        /// Do this outside of the process block, as it does not need to run at sample rate.  
        /// </summary>
        /// 
        /// <param name="Frequency"></param>
        /// The desired cutoff frequency.
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.

        public void SetFilterParameters(double Frequency, int sample_rate)
        {
            onepolecoefficent = (float)Math.Exp((float)(-2.0 * Math.PI * Frequency / sample_rate));
            a0 = (float)1.0 - onepolecoefficent;
        }

        /// <summary>
        /// This is placed into the process block and contains the filter created by calling SetFrequency().   
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// The input sample.
        /// 
        /// <returns> The float value after passing through the filter. </returns>

        public float Filter(float inputSample)
        {
            return onepoleOut = inputSample * a0 + onepoleOut * onepolecoefficent;
        }
    }
}