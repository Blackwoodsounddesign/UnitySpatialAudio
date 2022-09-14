namespace AudioFXToolkitDSP
{
    /****************
     * Allpass Filter Class 
     * --------------
     *  An all pass filter built around a single circular delay line. It can be thought of as an input "smearer." 
     *  
     *  This can be used to create reverbs, phasors, and other interesting effects.
     */

    public class AllpassFilter
    {
        private DelayLine allpassDelayLine;
        private int sample_rate;

        /// <param name="m_sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        public void SetSampleRate(int m_sample_rate)
        {
            sample_rate = m_sample_rate;
            allpassDelayLine.SetSampleRate(sample_rate);
        }

        /// <summary>
        /// The constructor for the allpass filter.  
        /// </summary>
        /// <param name="maxDelaySamp"></param>
        public AllpassFilter(int maxDelaySamp = 10000)
        {
            allpassDelayLine = new DelayLine(maxDelaySamp);
        }

        /// <summary>
        /// This is the filter that makes the magic happen.
        /// </summary>
        /// <param name="inputSample"></param>
        /// The floating point input sample.
        /// 
        /// <param name="sampleDelay"></param>
        /// The sample delay for the allpass filter. The larger it is, the more smeared the input signal will be.
        /// 
        /// <param name="linearGain"></param>
        /// <returns> The float value after passing through the filter. </returns>
        public float Filter(float inputSample, float sampleDelay, float linearGain = 0.7f)
        {
            allpassDelayLine.WriteDelay(inputSample + (linearGain * allpassDelayLine.SampleDelay(sampleDelay)));
            return (inputSample * -1 * linearGain) + (linearGain * allpassDelayLine.SampleDelay(sampleDelay));
        }

    }
}
