using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    [SerializeField] bool delayOnOff;

    [Range(0f, 1f)]
    public float gainL;
    [Range(0f, 1f)]
    public float gainR;

    [Header("Delay Settings")]
    [Range(0.05f, 2f)]
    public float delayTime;
    public float delgain;

    int sr; 

    DelayLine delayLinel = new DelayLine();
    DelayLine delayLiner = new DelayLine();



    private void Start()
    {
        sr = AudioSettings.outputSampleRate;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //makes sure the audio is stereo
        if (channels != 2)
            return;

        int dataLen = data.Length;

        int n = 0;

        //process block, this is interleved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (delayOnOff)
            {
                if (channeliter == 0)
                {
                    delayLinel.WriteDelay(data[n]);
                    data[n] = data[n] * gainL + delayLinel.DelayTap(sr, delayTime) * delgain;
                }
                else
                {
                    delayLiner.WriteDelay(data[n]);
                    data[n] = data[n]*gainR + delayLiner.DelayTap(sr, delayTime) * delgain;
                }
            }

            n++;
        }

    }

    class DelayLine
    {
        readonly float[] delaymem = new float[2098152];
        int writePointer;
        float readPointer;

        public DelayLine()
        {
            for (int i = 0; i < 2098151; i++)
            {
                delaymem[i] = 0f;
            }
        }

        public void WriteDelay(float inputsamp)
        {

            //write to the delaybuffer
            delaymem[writePointer] = inputsamp;
            writePointer++;

            if (writePointer > 2098151)
                writePointer = 0;
        }

        //in MSEC
        public float DelayTap(int m_sampleRate, float delTime)
        {
            float tapout;
            float delaytrail = (float)(delTime * m_sampleRate);

            //the readpointer
            if (writePointer < delaytrail)
                readPointer = (2098151 - delaytrail) + writePointer;
            else
                readPointer = ((float)(writePointer - delaytrail));

            //finds the decimal part of the readpointer
            int readpointertrunc = (int)readPointer;
            float delta = readPointer - readpointertrunc;

            //calculates the fractional part of the delay through linear interpolation
            if (readpointertrunc == 0)
                tapout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[2098151]);
            else
                tapout = ((1 - delta) * delaymem[readpointertrunc]) + (delta * delaymem[readpointertrunc - 1]);

            return tapout;
        }

    }

}
