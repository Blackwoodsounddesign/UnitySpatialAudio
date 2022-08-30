using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeakNotch : MonoBehaviour
{
    //gain control
    [Range(0.0f, 1.0f)]
    public float gainL = 1.0f;
    [Range(0.0f, 1.0f)]
    public float gainR = 1.0f;

    [SerializeField] private bool PeakNotchOnOff;

    [Range(20f, 20000f)]
    public float frequency = 1000f;
    [Range(-20f, 20f)]
    public float dB = 0f;

    BlueShiftDSP.PeakNotch peaknotchl = new BlueShiftDSP.PeakNotch();
    BlueShiftDSP.PeakNotch peaknotchr = new BlueShiftDSP.PeakNotch();

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

        peaknotchl.SetFilterParameters(sr,frequency, dB);
        peaknotchr.SetFilterParameters(sr, frequency, dB);

        //process block, this is interleved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (PeakNotchOnOff)
            {
                if (channeliter == 0)
                    data[n] = gainL * peaknotchl.Filter(data[n]);
                else
                    data[n] = gainR * peaknotchr.Filter(data[n]);
            }

            n++;
        }

    }
}
