using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tremelo : MonoBehaviour
{
    [Range(0f, 1f)]
    public float gainL = 0.75f;
    [Range(0f, 1f)]
    public float gainR = 0.75f;

    [Header("Wavetable Settings")]
    [Range(0.05f, 2f)]
    public float depthL = 0.5f;
    [Range(0.0f, 2f)]
    public float depthR = 0.5f;
    [Range(1f, 50f)]
    public float Rate = 100f;

    float sr;

    [SerializeField] private bool TremeloOnOff;

    BlueShiftDSP.Wavetable wavetablel = new BlueShiftDSP.Wavetable();
    BlueShiftDSP.Wavetable wavetabler = new BlueShiftDSP.Wavetable();

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

            if (TremeloOnOff)
            {
                if (channeliter == 0)
                {
                    data[n] = data[n] * (depthL * Math.Abs(wavetablel.WavetableProcess(Rate, sr)));
                }
                else
                {
                    data[n] = data[n] * (depthR * Math.Abs(wavetabler.WavetableProcess(Rate, sr)));
                }
            }

            n++;
        }

    }
}
