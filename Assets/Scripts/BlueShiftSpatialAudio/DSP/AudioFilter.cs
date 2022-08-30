using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFilter : MonoBehaviour
{

    //gain control
    [Range (0.0f, 1.0f)]
    public float gainL;
    [Range(0.0f, 1.0f)]
    public float gainR;

    [Header ("Lowpass Filter")]
    //onepole filter
    [Range(0.0f, 20000f)]
    public float filterF;

    BlueShiftDSP.Onepole onepolel = new BlueShiftDSP.Onepole();
    BlueShiftDSP.Onepole onepoler = new BlueShiftDSP.Onepole();
    int sr;

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

        onepolel.SetFrequency(filterF, sr);
        onepoler.SetFrequency(filterF, sr);

        //process block, this is interleved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (channeliter == 0)
                data[n] = gainL * onepolel.Filter(data[n]);
            else
                data[n] = gainR * onepoler.Filter(data[n]);

            n++;
        }

    }
}