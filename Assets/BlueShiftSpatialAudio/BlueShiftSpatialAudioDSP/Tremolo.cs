using System;

namespace AudioFXToolkitDSP
{
    /**
     * Tremolo
     * ----------
     * A classic tremolo effect. This is done be modulating the input audio signal by an oscillator. This is known as Amplitude modulation. 
     * In this case, the user is given control over the frequency rate and ampitude of the oscillator.
     * 
     * https://en.wikipedia.org/wiki/Tremolo_(electronic_effect) 
     */

    public class Tremolo
    {

        private Wavetable wavetable;

        private float m_rate = 1f;
        private float m_depth = 0f;
        private float DCoffset = 0f;

        /// <summary>
        /// The constructor of the tremolo. To do this we need an oscillator, so we create a Wavetable object.   
        /// </summary>
        public Tremolo() => wavetable = new Wavetable();

     
        /// <summary>
        /// Sets the parameters of the Tremolo. Call this outside of the process block. 
        /// </summary>
        /// 
        /// <param name="Rate"></param>
        /// The tremolo rate.
        /// 
        /// <param name="Depth"></param>
        /// The depth of the tremolo signal.
        /// 
        /// <param name="sample_rate"></param>
        /// Sets the sample rate. 

        public void SetTremoloParams(float Rate, float Depth, int sample_rate)
        {
            m_rate = Rate;
            m_depth = Depth;
            DCoffset = 1 - m_depth;
            wavetable.SetSampleRate(sample_rate);
        }

        /// <summary>
        /// Tremolo.Effect goes into the process block. This does the DSP for you.  
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// <returns> The float value after passing through the tremolo. </returns>

        public float Effect(float inputSample)
        {
            return inputSample * (DCoffset + (m_depth * Math.Abs(wavetable.WavetableProcess(m_rate / 2f))));
        }
    }
}
