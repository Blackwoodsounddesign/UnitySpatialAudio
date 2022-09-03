using System;
using UnityEngine;

public class RMStest : MonoBehaviour
{
    private RMS rms = new RMS();
    private RMS rms2 = new RMS();

    //gain control
    [Range(0.0f, 1.0f)]
    public float gainL;
    [Range(0.0f, 1.0f)]
    public float gainR;

    public float rmsvalue;
    public float rmsvalue2;

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

            if (channeliter == 0)
            {
                data[n] = data[n];
                rmsvalue = rms.GetRMS(data[n]);
            }
            else
            {
                data[n] = data[n];
                rmsvalue2 = rms2.GetRMS(data[n]);
            }

            n++;
        }
    }

}



public class RMS
{
    private int envPosition;
    private int envArraySize = 64;
    private float envArrayTotal;
    private float[] envArray;

    public RMS()
    {
        envArray = new float[envArraySize];

        for (int i = 0; i < envArraySize - 1; i++)
            envArray[i] = 0f;
    }


    public float GetRMS(float sample)
    {
        float square;
        float mean;

        // wrap the index pointer   
        if (envPosition >= envArraySize)
            envPosition = 0;
        if (envPosition < 0)
            envPosition = 0;

        // square
        square = sample * sample;

        // calculate the mean   
        envArrayTotal = envArrayTotal - envArray[envPosition] + square;
        envArray[envPosition] = square;
        envPosition++;

        // THIRD: mean is total/arraysize    
        mean = envArrayTotal / (float)envArraySize;

        //  square root of mean    
        return ((float)Math.Sqrt(mean));
    }
}