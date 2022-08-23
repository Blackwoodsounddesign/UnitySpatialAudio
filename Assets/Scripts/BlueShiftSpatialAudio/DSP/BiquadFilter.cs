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

    Biquad biquadl = new Biquad();
    Biquad biquadr = new Biquad();

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
                    data[n] = gainL * biquadl.BiQuad(data[n]);
                else
                    data[n] = gainR * biquadr.BiQuad(data[n]);
            }

            n++;
        }

    }

    //Biquad DSP
    class Biquad
    {
        private float a0, a1, a2, b1, b2;
        private float x1, x2, y1, y2;

        public float BiQuad(float samp)
        {
            //Biquad 
            float result = a0 * samp + a1 * x1 + a2 * x2 - b1 * y1 - b2 * y2;

            // shift x1 to x2, sample to x1
            x2 = x1;
            x1 = samp;

            // shift y1 to y2, result to y1
            y2 = y1;
            y1 = result;

            return result;
        }

        //setter 
        public void SetCoefficents(float _a0, float _a1, float _a2, float _b1, float _b2)
        {
            a0 = _a0; a1 = _a1; a2 = _a2; b1 = _b1; b2 = _b2;
        }
    }
}
