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

    Onepole onepolel = new Onepole();
    Onepole onepoler = new Onepole();
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

        onepolel.SetFc(filterF, sr);
        onepoler.SetFc(filterF, sr);

        //process block, this is interleved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (channeliter == 0)
                data[n] = gainL * onepolel.OnePole(data[n]);
            else
                data[n] = gainR * onepoler.OnePole(data[n]);

            n++;
        }

    }
}

//onepole DSP
class Onepole
{
    float opout = 0;
    float a0 = 1.0f;
    float opcoef = 0;

    public void SetFc(double Fc, int sr)
    {
        opcoef = Mathf.Exp((float)(-2.0 * Mathf.PI * Fc / sr));
        a0 = (float)1.0 - opcoef;
    }

    public float OnePole(float samp)
    {
        return opout = samp * a0 + opout * opcoef;
    }
}