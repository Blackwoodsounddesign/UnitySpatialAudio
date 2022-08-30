using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saturation : MonoBehaviour
{
    [Range(0f, 1f)]
    public float gainL = 0.75f;
    [Range(0f, 1f)]
    public float gainR = 0.75f;

    [Header("Wavetable Settings")]
    [Range(0.05f, 10f)]
    public float Gain = 0.5f;

    [SerializeField] private bool DistortionOnOff;
    readonly BlueShiftDSP.Distort distortl = new BlueShiftDSP.Distort();
    readonly BlueShiftDSP.Distort distortr = new BlueShiftDSP.Distort();

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

            if (DistortionOnOff)
            {
                if (channeliter == 0)
                    data[n] = distortl.Soft(data[n], Gain);
                else
                    data[n] = distortr.Soft(data[n], Gain);
            }

            n++;
        }

    }
}
