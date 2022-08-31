using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Allpass : MonoBehaviour
{
    //gain control
    [Range(0.0f, 1.0f)]
    public float gainL;
    [Range(0.0f, 1.0f)]
    public float gainR;

    [Header("Allpass Filter")]
    //onepole filter
    [Range(0.0f, 10000f)]
    public float sampleDelay = 10f;
     
    BlueShiftDSP.AllpassFilter allpassl = new BlueShiftDSP.AllpassFilter();
    BlueShiftDSP.AllpassFilter allpassr = new BlueShiftDSP.AllpassFilter();

    private void Awake()
    {
        allpassl.SetSampleRate(AudioSettings.outputSampleRate);
        allpassr.SetSampleRate(AudioSettings.outputSampleRate);
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

            if (channeliter == 0)
                data[n] = gainL * allpassl.Filter(data[n], sampleDelay);
            else
                data[n] = gainR * allpassr.Filter(data[n], sampleDelay);

            n++;
        }

    }
}
