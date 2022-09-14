using System;

namespace AudioFXToolkitDSP
{

    /***********
     * SimpleVerb
     * ------------
     * The simplest reverb worth anything is this Schroeder verb. Generally, algorithmic reverbs are done by using this method or a feedback delay network.
     * Creating a FDN or Freeverb style reverb would be a good first project when expirmenting with these algorithms. <br>
     * https://ccrma.stanford.edu/~jos/pasp/Schroeder_Reverberators.html
     */

    public class SimpleVerb
    {
        private AllpassFilter[] allpassFilters;
        private FeedBackCombFilter[] feedBackCombFilters;
        private SimpleFilter simpleFilter;

        private float m_WetGain;
        private float Sample;
        private float Sample2;
        private float Sample3;

        /// <summary>
        /// The constructor of the reverb. This creates 3 AllpassFilter objects and 4 FeedbackCombFilter objects. 
        /// </summary>
        
        public SimpleVerb()
        {
            feedBackCombFilters = new FeedBackCombFilter[4];
            allpassFilters = new AllpassFilter[3];
            simpleFilter = new SimpleFilter();

            // creates 4 FeedBackCombFilter objects
            for (int i = 0; i < 4; i++)
            {
                feedBackCombFilters[i] = new FeedBackCombFilter();
            }

            // creates 3 AllpassFilter objects
            for (int i = 0; i < 3; i++)
            {
                allpassFilters[i] = new AllpassFilter();
            }
        }

        /// <summary>
        /// This function sets the parameters of the reverb. Call it before running the reverb in the process block.  
        /// </summary>
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        /// 
        /// <param name="absorptionFilterFrequency"></param>
        /// A onepole filter that simulates darkening the room. This uses the SimpleFilter object. 
        /// 
        /// <param name="WetGain"></param>
        /// 

        public void SetSimpleVerbParams(int sample_rate, float absorptionFilterFrequency, float WetGain)
        {
            // set the sample rate of all of the allpass filters
            for (int i = 0; i < 3; i++)
            {
                allpassFilters[i].SetSampleRate(sample_rate);
            }

            // set the sample rate of all of the feedBackCombFilters filters
            for (int i = 0; i < 4; i++)
            {
                feedBackCombFilters[i].SetSampleRate(sample_rate);
            }

            simpleFilter.SetFilterParameters(absorptionFilterFrequency, sample_rate);
            m_WetGain = WetGain; 
        }

        /// <summary>
        /// This goes into the process block and preforms the reverb algorithm.
        /// Changing the delay values hard coded in, will have drastic effects on the sound of the reverb.
        /// These values are known to sound decent, but other values are worth playing with. 
        /// </summary>
        /// <param name="inputSample"></param>
        /// <returns> The sample passed through the reverb. </returns>
        public float Effect(float inputSample)
        {
            Sample = allpassFilters[0].Filter(inputSample, 695);
            Sample2 = allpassFilters[1].Filter(Sample, 226);
            Sample3 = allpassFilters[2].Filter(Sample2, 73);

            float fbSample1 = feedBackCombFilters[0].Filter(Sample3, 3375);
            float fbSample2 = feedBackCombFilters[1].Filter(Sample3, 3201);
            float fbSample3 = feedBackCombFilters[2].Filter(Sample3, 2105);
            float fbSample4 = feedBackCombFilters[3].Filter(Sample3, 2503);

            return simpleFilter.Filter(m_WetGain * (fbSample1 + fbSample2 + fbSample3 + fbSample4)/2); 
        }
    }



}
