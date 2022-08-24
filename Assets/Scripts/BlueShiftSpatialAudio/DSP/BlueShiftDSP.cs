﻿using System;

namespace BlueShiftDSP
{


    //To Do: Port the wavetable and add an allpass, RMS averager, soft/clip distortion. 

    /****************
     * Biquad Class 
     * --------------
     * This is the basic biquad function with a setter and the generic DSP. 
     *
     * To Do: Port RBJ filters from Gen/Max. 
     */

    public class Biquad
    {
        private float a0, a1, a2, b1, b2;
        private float x1, x2, y1, y2;

        public float BiQuad(float samp)
        {
            // That sweet Biquad code.  
            float result = a0 * samp + a1 * x1 + a2 * x2 - b1 * y1 - b2 * y2;

            // shift x1 to x2, sample to x1
            x2 = x1;
            x1 = samp;

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
     *  It can be turned into a highpass filter. 
     */

    public class Onepole
    {
        private float opout = 0;
        private float a0 = 1.0f;

        private float opcoef = 0;
        public void SetOpCoef(int i)
        {
            if(i==0 || i == 1)
                opcoef = i;
        }

        public void SetFc(double Fc, int sr)
        {
            opcoef = (float)Math.Exp((float)(-2.0 * Math.PI * Fc / sr));
            a0 = (float)1.0 - opcoef;
        }

        public float Filter(float samp)
        {
            return opout = samp * a0 + opout * opcoef;
        }
    }


    /****************
     * Delay Class 
     * --------------
     * A mono delay line. 
     * 
     * It contains a tap delay, sample delay, and a feedback delay. 
     * All of the delays are setup as fractional delay lines. 
     */

    public class DelayLine
    {
        private float[] delaymem; 
        int writePointer;
        float readPointer;

        private int buffersize;
        public int GetBufferSize() => buffersize; 

        public DelayLine(int m_buffersize = 144000)
        {
            buffersize = m_buffersize;
            delaymem = new float[buffersize];

            for (int i = 0; i < buffersize - 1; i++)
            {
                delaymem[i] = 0f;
            }
        }

        public void WriteDelay(float inputsamp)
        {
            //write to the delaybuffer
            delaymem[writePointer] = inputsamp;
            writePointer++;

            if (writePointer > buffersize - 1)
                writePointer = 0;
        }

        //Delay in MSEC
        public float DelayTap(int m_sampleRate, float delTime)
        {
            float tapout;
            float delaytrail = Math.Clamp((float)(delTime * m_sampleRate), 0, buffersize);

            //the readpointer
            if (writePointer < delaytrail)
                readPointer = buffersize - delaytrail - 1 + writePointer;
            else
                readPointer = writePointer - delaytrail;

            //finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            //calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                tapout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[buffersize - 1]);
            else
                tapout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[readpointertrunc - 1]);

            return tapout;
        }

        //Delay in samples
        public float SampleDelay(int delSample)
        {
            float tapout;
            float delaytrail = Math.Clamp(delSample, 0, buffersize);

            //the readpointer
            if (writePointer < delaytrail)
                readPointer = buffersize - delaytrail - 1 + writePointer;
            else
                readPointer = writePointer - delaytrail;

            //finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            //calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                tapout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[buffersize - 1]);
            else
                tapout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[readpointertrunc - 1]);

            return tapout;
        }

        //Feedback delay
        public float FeedBackDelay(float inputsamp, int m_sampleRate, float delTime, float feedbackamount)
        {
            float delaytrailf = delTime * m_sampleRate;
            int bufferMinus1 = buffersize - 1;
            float delayout;

            //the readpointer
            if (writePointer < delaytrailf)
                readPointer = bufferMinus1 - delaytrailf + writePointer;
            else
                readPointer = writePointer - delaytrailf;

            //finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            //calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                delayout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[bufferMinus1]);
            else
                delayout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[readpointertrunc - 1]);


            //write to the delaybuffer
            delaymem[writePointer] = inputsamp + (delayout * feedbackamount);
            writePointer++;

            if (writePointer > bufferMinus1)
                writePointer = 0;

            return delayout;
        }
    }
}
