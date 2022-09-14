using System;
namespace AudioFXToolkitDSP
{
    /****************
     * DelayLine Class 
     * --------------
     * A mono circular delay line. 
     * 
     * It contains a tap delay, sample delay, and a feedback delay. 
     * All of the delays use a linear interpolation to achieve fractional delay. This means the value of the delay can be in-between two samples.
     * 
     */

    public class DelayLine
    {
        // the delay buffer. AKA (where the magic happens)  
        private float[] delayMemory;
        int sample_rate;

        /// <summary>
        /// Sets the sample rate of the delay line. Call this on Awake() in Unity and pass in AudioSettings.outputSampleRate. 
        /// </summary>
        /// <param name="m_sample_rate"></param>
        /// Sets the sample rate. 

        public void SetSampleRate(int m_sample_rate) => sample_rate = m_sample_rate;

        int writePointer;
        float readPointer;

        private int bufferSize;
        public int GetBufferSize() => bufferSize;

        /// <summary>
        /// The constructor for the delay line. 
        /// </summary>
        /// 
        /// <param name="m_buffersize"></param>
        /// Change the size of this buffer to allocate only the memory needed.
        /// This is a long delay (3 seconds at a samplerate of 48000).

        public DelayLine(int m_buffersize = 144000)
        {
            bufferSize = m_buffersize;
            delayMemory = new float[bufferSize];

            for (int i = 0; i < bufferSize - 1; i++)
            {
                delayMemory[i] = 0f;
            }
        }

        /// <summary>
        /// This writes to the delay buffer sample-by-sample. 
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// The inputsamp is the float value being placed into the delay line.
        /// This is taken from an audio source. 

        public void WriteDelay(float inputSample)
        {
            // write to the delaybuffer
            delayMemory[writePointer] = inputSample;
            writePointer++;

            if (writePointer > bufferSize - 1)
                writePointer = 0;
        }

        /// <summary>
        /// A milisecond circular buffer tap delay.
        /// To use this, first write to the delay line using WriteDelay().
        /// DelayTap() will return the float value at the desired millisecond value.
        /// You can call DelayTap() multiple times, but you should only write to the delay line once per channel.
        /// SampleDelay() and DelayTap() read from the same WriteDelay() buffer, so you can use them interchangeably. 
        /// </summary>
        /// 
        /// <param name="m_sampleRate"></param>
        /// The sample rate of the delay buffer.
        /// 
        /// <param name="delayTime"></param>
        /// This is in milliseconds. Under the hood, it does the conversion to samples.
        /// 
        /// <returns> The sample at the desired delay time. </returns>

        public float DelayTap(float delayTime)
        {
            float tapout;
            float delaytrail = Math.Clamp((float)(delayTime * sample_rate), 0, bufferSize);

            // the readpointer
            if (writePointer < delaytrail)
                readPointer = bufferSize - delaytrail - 1 + writePointer;
            else
                readPointer = writePointer - delaytrail;

            // finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            // calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                tapout = ((1 - delta) * delayMemory[readpointertrunc]) + (delta * delayMemory[bufferSize - 1]);
            else
                tapout = ((1 - delta) * delayMemory[readpointertrunc]) + (delta * delayMemory[readpointertrunc - 1]);

            return tapout;
        }

        /// <summary>
        /// A sample based circular buffer tap delay. This is useful for creating filters and reflections.
        /// /// To use this, first write to the delay line using WriteDelay().
        /// SampleDelay() will return the float value at the desired delayed sample value.
        /// You can call SampleDelay() multiple times, but you should only write to the delay line once per channel.
        /// SampleDelay() and DelayTap() read from the same WriteDelay() buffer, so you can use them interchangeably. 
        /// </summary>
        /// 
        /// <param name="numberOfSamples"></param>
        /// How many samples to delay the input by. 
        /// 
        /// <returns>The sample at the desired sample delay.</returns>

        public float SampleDelay(float numberOfSamples)
        {
            float tapout;
            float delayTrail = Math.Clamp(numberOfSamples, 0, bufferSize);

            // the readpointer
            if (writePointer < delayTrail)
                readPointer = bufferSize - delayTrail - 1 + writePointer;
            else
                readPointer = writePointer - delayTrail;

            // finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            // calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                tapout = ((1 - delta) * delayMemory[readpointertrunc]) + (delta * delayMemory[bufferSize - 1]);
            else
                tapout = ((1 - delta) * delayMemory[readpointertrunc]) + (delta * delayMemory[readpointertrunc - 1]);

            return tapout;
        }

        /// <summary>
        /// A mono delay line with feedback.
        /// This feeds back onto self and creates an echo effect.
        /// A value of 1 makes the delay last forever and greater than one makes it explode. 
        /// \warning
        /// Do not write to the delay line when calling this method. This has a built in WriteDelay with code added for feedback. 
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// The inputsamp is the float value being placed into the delay line.
        /// This is taken from an audio source.
        /// 
        /// <param name="m_sampleRate"></param>
        /// The sample rate of the delay buffer.
        /// 
        /// <param name="delayTime"></param>
        /// This is in miliseconds. Under the hood, it does the conversion to samples.
        /// 
        /// <param name="feedbackAmount"></param>
        /// A value of one creates an infinite delay. More will cause the delay to "explode" and less causes a slow decay.
        /// 
        /// <returns>The sample at the desired sample delay.</returns>
        ///

        public float FeedBackDelay(float inputSample, float delayTime, float feedbackAmount)
        {
            float delaytrailf = Math.Clamp(delayTime * sample_rate, 0, bufferSize);
            int bufferMinus1 = bufferSize - 1;
            float delayout;

            // the readpointer
            if (writePointer < delaytrailf)
                readPointer = bufferMinus1 - delaytrailf + writePointer;
            else
                readPointer = writePointer - delaytrailf;

            // finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            // calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                delayout = ((1 - delta) * delayMemory[readpointertrunc]) + (delta * delayMemory[bufferMinus1]);
            else
                delayout = ((1 - delta) * delayMemory[readpointertrunc]) + (delta * delayMemory[readpointertrunc - 1]);


            // write to the delaybuffer
            delayMemory[writePointer] = inputSample + (delayout * feedbackAmount);
            writePointer++;

            if (writePointer > bufferMinus1)
                writePointer = 0;

            // output
            return delayout;
        }
    }

}
