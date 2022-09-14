namespace AudioFXToolkitDSP
{

    /****************
     * Feed Foward Comb Filter 
     * --------------
     *  A feedfoward Comb Filter built around a single circular delay line utilizing only the input (vs feedback utilizing the output).
     *  This is useful for simulating spaces, create reverbs, phasors, and other interesting effects.
     */

    public class FeedFowardCombFilter
    {
        DelayLine feedfowardDelayLine;

        /// <summary>
        /// The constructor for the FeedFowardCombFilter.
        /// </summary>
        /// 
        /// <param name="maxDelaySamp"></param>
        /// The highest number of samples the comb filter will delay by. 

        public FeedFowardCombFilter(int maxDelaySamp = 10000)
        {
            feedfowardDelayLine = new DelayLine(maxDelaySamp);
        }

        /// <summary>
        /// This is a useful structure in reverbs and other acoustic modeling. 
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
            // write to the delay line. 
            feedfowardDelayLine.WriteDelay(inputSample);

            // the comb filter dsp.   
            return (inputSample * inputCoefficent) + (feedfowardDelayLine.SampleDelay(delayInSamples) * delayCoefficent);
        }

    }
}
