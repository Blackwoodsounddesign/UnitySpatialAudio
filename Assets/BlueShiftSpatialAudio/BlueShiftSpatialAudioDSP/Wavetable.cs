using System;

namespace AudioFXToolkitDSP
{
    /**************
     * Wavetable
     * --------------
     * This class can be set up to create any kind of repetitive signal but is mostly used to create oscillators. 
     * These signals can be used as modulators in phasors, tremolos, stored signals in ffts, etc.  
     * 
     * This works by generating a buffer with one cycle of a desired signal. Then this buffer is indexed by a phasor at various rates.
     * The faster the buffer is indexed, the higher pitched the signal is. <br>
     * https://en.wikipedia.org/wiki/Wavetable_synthesis
     */

    public class Wavetable
    {
        // the table variables. 
        public double[] waveTableBuffer;
        readonly int tableSize;

        // variables to index the table with to create the oscillator.   
        private double frequency;
        private double oneOverSampleRate = 0;
        public double phasor = 0;
        private double index;
        private int sample_rate;
        public void SetSampleRate(int m_sample_rate) => sample_rate = m_sample_rate;

        /// <summary>
        /// The construstor for the wavetable takes in an optional table size. This allows large wavetable.
        /// An update will allow the user to pass in their own wavetables as an array of samples. 
        /// </summary>
        /// 
        /// <param name="m_tablesize"></param>

        public Wavetable(int m_tablesize = 1024)
        {
            // create the blank buffer which will be filled with a single sine wave. 
            tableSize = m_tablesize;
            waveTableBuffer = new double[m_tablesize];

            // fill up the table buffer
            for (int i = 0; i < tableSize; i++)
                waveTableBuffer[i] = Math.Sin(Math.PI * i * 2f / tableSize);

        }

        /// <summary>
        /// WavetableProcess generates an oscillator at a user defined frequency.
        /// </summary>
        /// 
        /// <param name="m_frequency"></param>
        /// The rate per second the table is interpolated through.
        /// 
        /// <param name="sample_rate"></param>
        /// <returns>Audio sample float values.</returns>

        public float WavetableProcess(float m_frequency)
        {
            frequency = m_frequency;
            oneOverSampleRate = 1f / (float)sample_rate;

            // the output variable is allocated here to keep it in scope
            double waveout;

            // creating the phaser
            if (phasor + (oneOverSampleRate * frequency) <= 1)
                phasor += oneOverSampleRate * frequency;
            else
                phasor += -1.0f + (oneOverSampleRate * frequency);

            // variables for linear interpolation
            index = phasor * (double)tableSize;
            int indextrunc = (int)index;
            double delta = index - indextrunc;

            // linear interpolation
            if (indextrunc == tableSize - 1)
                waveout = ((1 - delta) * waveTableBuffer[indextrunc]) + (delta * waveTableBuffer[0]);
            else
                waveout = ((1 - delta) * waveTableBuffer[indextrunc]) + (delta * waveTableBuffer[indextrunc + 1]);


            // output
            return (float)waveout;
        }

    }
}
