using System;

namespace AudioFXToolkitDSP
{
    /***
     * RMS
     * --------
     * This returns the root-mean-square of a desired signal. Generally, RMS can be thought of as a good way of measuring loudness.
     * This can be used for compressors, limiters, multiband dynamics, etc. <br>
     * https://en.wikipedia.org/wiki/Root_mean_square
     */

    public class RMS
    {
        private int envPosition;
        private int envArraySize;
        private float envArrayTotal;
        private float[] envArray;

        /// <summary>
        /// The constructor for the RMS measurement. 
        /// </summary>
        /// <param name="m_envArraySize"></param>
        /// The size of the buffer to measure the RMS value of. The smaller the buffer, the quicker the changes will be. 

        public RMS(int m_envArraySize = 64)
        {
            envArraySize = m_envArraySize;

            envArray = new float[envArraySize];

            for (int i = 0; i < envArraySize - 1; i++)
                envArray[i] = 0f;
        }


        /// <param name="inputSample"></param>
        /// The input sample of the signal that is meant to be measured. 
        /// 
        /// <returns> The float value of the RMS measurement. </returns>

        public float GetRMS(float inputSample)
        {
            float square;
            float mean;

            // wrap the index pointer   
            if (envPosition >= envArraySize)
                envPosition = 0;
            if (envPosition < 0)
                envPosition = 0;

            // square
            square = inputSample * inputSample;

            // calculate the mean   
            envArrayTotal = envArrayTotal - envArray[envPosition] + square;
            envArray[envPosition] = square;
            envPosition++;

            // mean is total/arraysize    
            mean = envArrayTotal / (float)envArraySize;

            //  square root of the mean    
            return ((float)Math.Sqrt(mean));
        }
    }
}
