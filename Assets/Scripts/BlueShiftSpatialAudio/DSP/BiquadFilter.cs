using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiquadFilter : MonoBehaviour
{
    //gain control
    [Range(0.0f, 1.0f)]
    public float gainL = 1.0f;
    [Range(0.0f, 1.0f)]
    public float gainR = 1.0f;

    [SerializeField] private bool BiquadOnOff;  

    BlueShiftDSP.Biquad biquadl = new BlueShiftDSP.Biquad();
    BlueShiftDSP.Biquad biquadr = new BlueShiftDSP.Biquad();

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //makes sure the audio is stereo
        if (channels != 2)
            return;

        int dataLen = data.Length;

        int n = 0;

        biquadl.SetCoefficents(0.0535f, 0, -0.05355f, -1.8707f, 0.88263f);
        biquadr.SetCoefficents(0.0535f, 0, -0.05355f, -1.8707f, 0.88263f);

        //process block, this is interleved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (BiquadOnOff)
            {
                if (channeliter == 0)
                    data[n] = gainL * biquadl.Filter(data[n]);
                else
                    data[n] = gainR * biquadr.Filter(data[n]);
            }

            n++;
        }

    }
}