using System;
using UnityEngine;

public class Tremelo : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float gainL = 0.75f;
    [Range(0f, 1f)]
    [SerializeField] private float gainR = 0.75f;

    [Header("Tremelo Settings")]
    [Range(0.05f, 2f)]
    [SerializeField] private float depthL = 0.5f;
    [Range(0.0f, 2f)]
    [SerializeField] private float depthR = 0.5f;
    [Range(0.5f, 20f)]
    [SerializeField] private float Rate = 10f;


    [SerializeField] private bool TremeloOnOff;

    BlueShiftDSP.Wavetable wavetablel = new BlueShiftDSP.Wavetable();
    BlueShiftDSP.Wavetable wavetabler = new BlueShiftDSP.Wavetable();

    private void Awake()
    {
        wavetablel.SetSampleRate(AudioSettings.outputSampleRate);
        wavetabler.SetSampleRate(AudioSettings.outputSampleRate);
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
                    data[n] = data[n] * (depthL * Math.Abs(wavetablel.WavetableProcess(Rate/2f)));
                else
                    data[n] = data[n] * (depthR * Math.Abs(wavetabler.WavetableProcess(Rate/2f)));
            }

            n++;
        }

    }
}
