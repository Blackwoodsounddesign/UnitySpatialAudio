using System;


/// <summary>
/// BlueShiftDSP is the namespace holding the guts of the DSP.
/// 
/// The algorithms include:
/// 
///     Two distortion algorithms,
///     a delay line with a number of implementations,
///     a biquad filtering suite,       Partially done. 
///     a onepole filter,
///     an allpass filter,              To Do.
///     a comb filter,                  To Do. 
///     a sample peak/rms detector,     To Do.
///     and a wavetable.  
/// 
/// All of this code has been tested and known to work. Change it at your own peril. 
/// </summary>


namespace BlueShiftDSP
{

    /****************
     * Distortion Class 
     * --------------
     * Two simple distortion algorithms.
     */

    public class Distort
    {
        /// <summary>
        /// This is a simple clip distortion algorithm. Anything above 1 or below -1 is clipped to those values.
        /// This creates a distortion accentuating the higher frequencies.
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// The input sample.
        /// 
        /// <param name="gain"></param>
        /// The predistortion gain.
        /// 
        /// <returns> The float value after passing through the distortion. </returns>
        
        public float Clip(float sample, float gain)
        { 
            return Math.Clamp(sample * gain, -1.0f, 1.0f);
        }

        /// <summary>
        /// This is a simple soft distortion algorithm. It does this by passing the input through the arctan algorithm. 
        /// This creates a distortion accentuating the mid frequencies.
        /// </summary>
        /// 
        /// <param name="sample"></param>
        /// The input sample.
        /// 
        /// <param name="gain"></param>
        /// The predistortion gain.
        /// 
        /// <returns> The float value after passing through the distortion. </returns>

        public float Soft(float sample, float gain)
        {
            return (float)((2/Math.PI) * Math.Atan(sample * gain));
        }
    }



    /****************
     * Biquad Class 
     * --------------
     * A generic DSP biquad function with a setter.
     * This is used throughout the various filters.
     */

    public class Biquad
    {
        private float a0, a1, a2, b1, b2;
        private float x1, x2, y1, y2;

        /// <summary>
        /// This is a biquad filter algorithm. Set the coeffiecents using any number of filter coefficent functions.
        /// Included in BlueShiftDSP is a number of these functions.
        ///
        ///     Included:
        ///     
        ///         Peaknotch
        ///         Bandpass
        ///         Lowpass
        ///         Highpass
        ///         Lowshelf
        ///         Highshelf
        ///
        /// </summary>
        /// 
        /// <param name="inputSample"></param>
        /// The input sample.
        /// 
        /// <returns> The float value after passing through the filter. </returns>

        public float Filter(float inputSample)
        {
            // That sweet Biquad code.  
            float result = a0 * inputSample + a1 * x1 + a2 * x2 - b1 * y1 - b2 * y2;

            // shift x1 to x2, sample to x1
            x2 = x1;
            x1 = inputSample;

            // shift y1 to y2, result to y1
            y2 = y1;
            y1 = result;

            return result;
        }

        // Coefficent setter.  
        public void SetCoefficents(float _a0, float _a1, float _a2, float _b1, float _b2)
        {
            a0 = _a0; a1 = _a1; a2 = _a2; b1 = _b1; b2 = _b2;
        }
    }



    /****************
     * OnePole Filter Class 
     * --------------
     *  A simple one pole filter.
     *  
     *  It can be turned into a highpass filter by subracting the filtered audio signal from the original audio signal. 
     */

    public class Onepole
    {

        private float onepoleOut = 0;
        private float a0 = 1.0f;
        private float onepolecoefficent = 0;

        /// <summary>
        /// This sets the frequency of the onepole filter.
        /// Do this outside of the process block, as it does not need to run at sample rate.  
        /// </summary>
        /// 
        /// <param name="Frequency"></param>
        /// The desired cutoff frequency.
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        
        public void SetFrequency(double Frequency, int sample_rate)
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



    /****************
     * Delay Class 
     * --------------
     * A mono circular delay line.
     * 
     * It contains a tap delay, sample delay, and a feedback delay. 
     * All of the delays use a linear interpolation to achieve fractional delay lines. 
     */

    public class DelayLine
    {
        // the delay buffer. AKA (where the magic happens)  
        private float[] delayMemory;

        int writePointer;
        float readPointer;

        private readonly int bufferSize;
        public int GetBufferSize() => bufferSize;

        /// <summary>
        /// The constructor for delay lines. 
        /// </summary>
        /// 
        /// <param name="m_buffersize"></param>
        /// Change the size of this buffer to only allocate the memory needed.
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
        /// This is called after the write delay.
        /// </summary>
        /// 
        /// <param name="m_sampleRate"></param>
        /// The sample rate of the delay buffer.
        /// 
        /// <param name="delayTime"></param>
        /// This is in miliseconds. Under the hood, it does the conversion to samples.
        /// 
        /// <returns> The sample at the desired delay time. </returns>

        public float DelayTap(int m_sampleRate, float delayTime)
        {
            float tapout;
            float delaytrail = Math.Clamp((float)(delayTime * m_sampleRate), 0, bufferSize);

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

        public float FeedBackDelay(float inputSample, int m_sampleRate, float delayTime, float feedbackAmount)
        {
            float delaytrailf = delayTime * m_sampleRate;
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



    /**************
     * Wavetable
     * --------------
     * A buffer set up to be indexed by a phasor. 
     * 
     * This class can be set up to create any kind of repetitive signal. 
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
        /// WavetableProcess generates an oscillator at a user defined frequency. Use it correctly get the audio sample rate
        /// from Unity and pass that in as well (standard is 48000). 
        /// </summary>
        /// 
        /// <param name="m_frequency"></param>
        /// The rate per second the table is interpolated through.
        /// 
        /// <param name="sample_rate"></param>
        /// <returns>Audio sample float values.</returns>

        public float WavetableProcess(float m_frequency, float sample_rate)
        {
                frequency = (float)m_frequency; 
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
                return (float) waveout;
        }

    }



    ///////////BIQUAD  FILTER  SECTION///////////



    /****************
     * PeakNotch Class 
     * --------------
     * A biquad based peaknotch filter.
     */

    public class PeakNotch : Biquad
    {
        /// <summary>
        /// The PeakNotch filter. 
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

        public void SetFilterParameters(int sample_rate, float frequency, float dBgain = 0f, float Q = 0.707f)
        {

            float K = (float)Math.Tan((Math.PI * frequency) / sample_rate);
            float V0 = (float)Math.Pow(10, dBgain / 20);

            if (V0 < 1)
                V0 = 1 / V0;

            //boost coefficents
            if (dBgain >= 0)
            {
                float b0 = (1 + ((V0 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);
                float b1 = (2 * (K * K - 1)) / (1 + ((1 / Q) * K) + K * K);
                float b2 = (1 - ((V0 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);
                float a0 = 1;
                float a1 = b1;
                float a2 = (1 - ((1 / Q) * K) + K * K) / (1 + ((1 / Q) * K) + K * K);

                SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);

            }
            else
            {
                float b0 = (1 + ((1 / Q) * K) + K * K) / (1 + ((V0 / Q) * K) + K * K);
                float b1 = (2 * (K * K - 1)) / (1 + ((V0 / Q) * K) + K * K);
                float b2 = (1 - ((1 / Q) * K) + K * K) / (1 + ((V0 / Q) * K) + K * K);
                float a1 = b1;
                float a0 = 1;
                float a2 = (1 - ((V0 / Q) * K) + K * K) / (1 + ((V0 / Q) * K) + K * K);

                SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);

            }
        }
    }


    /****************
     * Lowshelf Class 
     * --------------
     * A biquad based lowshelf filter.
     */

    public class Lowshelf : Biquad
    {
        /// <summary>
        /// The Lowshelf filter. 
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

        public void SetFilterParameters(int sample_rate, float frequency, float dBgain = 0f, float Q = 0.707f)
        {
            //intermediate
            float A = (float)Math.Pow(10, dBgain / 40);
            float w0 = (float)(2 * Math.PI * frequency / sample_rate);
            float alpha = (float)(Math.Sin(w0) * 0.5 * Math.Sqrt((A + 1 / A) * (1 / Q - 1) + 2));

            //coefficents from RBJ cookbook
            float b0 = (float)(A * ((A + 1) - (A - 1) * Math.Cos(w0) + 2 * Math.Sqrt(A) * alpha));
            float b1 = (float)(2 * A * ((A - 1) - (A + 1) * Math.Cos(w0)));
            float b2 = (float)(A * ((A + 1) - (A - 1) * Math.Cos(w0) - 2 * Math.Sqrt(A) * alpha));
            float a0 = (float)((A + 1) + (A - 1) * Math.Cos(w0) + 2 * Math.Sqrt(A) * alpha);
            float a1 = (float)(-2 * ((A - 1) + (A + 1) * Math.Cos(w0)));
            float a2 = (float)((A + 1) + (A - 1) * Math.Cos(w0) - 2 * Math.Sqrt(A) * alpha);

            SetCoefficents(b0 / a0, b1 / a0, b2 / a0, a1 / a0, a2 / a0);
        }
    }



    /****************
     * Highshelf Class 
     * --------------
     * A biquad based highshelf filter.
     */

    public class Highshelf : Biquad
    {
        /// <summary>
        /// The Highshelf filter. 
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



    /****************
     * Bandpass Class 
     * --------------
     * A biquad based highshelf filter.
     */

    public class Bandpass : Biquad
    {

        /// <summary>
        /// The Bandpass filter. 
        /// </summary>
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        /// 
        /// <param name="frequency"></param>
        /// The desired cutoff frequency. 
        /// 
        /// <param name="Q"></param>
        /// The filter Q.
        ///

        public void SetFilterParameters(int sample_rate, float frequency, float Q = 0.707f)
        {
            //intermediate
            float K = (float)Math.Tan(Math.PI * frequency / sample_rate);

            //boost coefficents
            float K2Q = (K * K * Q);

            float b0 = K / (K2Q + K + Q);
            float b1 = 0;
            float b2 = -K / (K2Q + K + Q);
            float a1 = (2 * Q * (K * K - 1)) / (K2Q + K + Q);
            float a2 = (K2Q - K + Q) / (K2Q + K + Q);

            SetCoefficents(b0, b1, b2, a1, a2);
        }
    }


    /****************
     * Lowpass Class 
     * --------------
     * A biquad based highshelf filter.
     */

    public class Lowpass : Biquad
    {

        /// <summary>
        /// The Lowpass filter. 
        /// </summary>
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        /// 
        /// <param name="frequency"></param>
        /// The desired cutoff frequency. 
        /// 
        /// <param name="Q"></param>
        /// The filter Q.
        ///

        public void SetFilterParameters(int sample_rate, float frequency, float Q = 0.707f)
        {
            //intermediate
            float K = (float)Math.Tan(Math.PI * frequency / sample_rate);

            //boost coefficents
            float K2Q = (K * K * Q);

            float b0 = K2Q / (K2Q + K + Q);
            float b1 = (2 * K2Q) / (K2Q + K + Q);
            float b2 = K2Q / (K2Q + K + Q);
            float a1 = (2 * Q * (K * K - 1)) / (K2Q + K + Q);
            float a2 = (K2Q - K + Q) / (K2Q + K + Q);

            SetCoefficents(b0, b1, b2, a1, a2);
        }
    }



    /****************
     * Highpass Class 
     * --------------
     * A biquad based highshelf filter.
     */

    public class Highpass : Biquad
    {

        /// <summary>
        /// The Lowpass filter. 
        /// </summary>
        /// 
        /// <param name="sample_rate"></param>
        /// The sample rate of the audio that is going to be filtered.
        /// 
        /// <param name="frequency"></param>
        /// The desired cutoff frequency. 
        /// 
        /// <param name="Q"></param>
        /// The filter Q.
        ///

        public void SetFilterParameters(int sample_rate, float frequency, float Q = 0.707f)
        {
            //intermediate
            float K = (float)Math.Tan(Math.PI * frequency / sample_rate);

            //boost coefficents
            float K2Q = (K * K * Q);

            float b0 = Q / (K2Q + K + Q);
            float b1 = (-2 * Q) / (K2Q + K + Q);
            float b2 = Q / (K2Q + K + Q);
            float a1 = (2 * Q * (K * K - 1)) / (K2Q + K + Q);
            float a2 = (K2Q - K + Q) / (K2Q + K + Q);

            SetCoefficents(b0, b1, b2, a1, a2);
        }
    }

}
