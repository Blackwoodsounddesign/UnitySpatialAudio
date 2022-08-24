using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    [SerializeField] bool delayOnOff;

    [Range(0f, 1f)]
    public float gainL = 0.75f;
    [Range(0f, 1f)]
    public float gainR = 0.75f;

    [Header("Delay Settings")]
    [Range(0.05f, 2f)]
    public float delayTime = 0.1f;
    [Range(0.0f, 1f)]
    public float delgain = 0.5f;
    [Range(0.0f, 1f)]
    public float Feedback = 0.5f;

    int sr;

    BlueShiftDSP.DelayLine delayLinel = new BlueShiftDSP.DelayLine();
    BlueShiftDSP.DelayLine delayLiner = new BlueShiftDSP.DelayLine();

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
                    //delayLinel.WriteDelay(data[n]);
                    data[n] = data[n] * gainL + delayLinel.FeedBackDelay(data[n],sr, delayTime,Feedback) * delgain;
                }
                else
                {
                    //delayLiner.WriteDelay(data[n]);
                    data[n] = data[n] * gainR + delayLiner.FeedBackDelay(data[n], sr, delayTime, Feedback) * delgain;
                }
            }

            n++;
        }

    }
}