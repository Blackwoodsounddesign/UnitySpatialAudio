namespace AudioFXToolkitDSP
{
    /****************
    * FeedbackCombFilter Class
    * --------------
    *  A feedback Comb Filter built around a single circular delay line.
    *  In this case, the delay line will feedback onto itself.
    *  
    *  This is useful for simulating spaces, reverbs, phasors, and other interesting effects.
    */

    public class FeedBackCombFilter
    {
        private DelayLine feedbackDelayLine;

        private int sample_rate;

        /// Sets the sample rate. 
        public void SetSampleRate(int m_sample_rate)
        {
            sample_rate = m_sample_rate;
            feedbackDelayLine.SetSampleRate(sample_rate);
        }

        /// <param name="maxDelaySamp"></param>
        /// The highest number of samples the comb filter will delay by. 

        public FeedBackCombFilter(int maxDelaySamp = 10000)
        {
            feedbackDelayLine = new DelayLine(maxDelaySamp);
        }

        /// <summary>
        /// This function goes into the process block. 
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// The signal that has the comb filter applied to it. 
        /// 
        /// <param name="delayInSamples"></param>
        /// The sample delay of the comb filter.
        ///
        /// <param name="inputCoefficent"></param>
        /// The input coefficent. A value of 1 is passed by default, but other values will increase/decrease the effect of the filter.
        ///  
        /// <param name="delayCoefficent"></param>
        /// The feedback coefficent. A value of 0.7 is passed by default, but other values will increase/decrease the effect of the filter.
        /// 
        /// <returns> The sample passed through the feedbackcomb filter. </returns>
        ///

        public float Filter(float inputSample, float delayInSamples, float inputCoefficent = 1f, float delayCoefficent = 0.7f)
        {
            // the comb filter dsp.   
            return feedbackDelayLine.FeedBackDelay(inputCoefficent * inputSample, delayInSamples / sample_rate, delayCoefficent);
        }

    }
}
