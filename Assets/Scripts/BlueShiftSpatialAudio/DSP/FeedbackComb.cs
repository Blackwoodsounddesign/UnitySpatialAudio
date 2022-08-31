using UnityEngine;
using System.Collections;

public class FeedbackComb : MonoBehaviour
{
    //gain control
    [Range(0.0f, 1.0f)]
    public float gainL;
    [Range(0.0f, 1.0f)]
    public float gainR;

    [Header("Comb Filter")]
    //onepole filter
    [Range(0.0f, 1000f)]
    public float delay;

    BlueShiftDSP.FeedBackCombFilter feedBackCombFilterl = new BlueShiftDSP.FeedBackCombFilter();
    BlueShiftDSP.FeedBackCombFilter feedBackCombFilterr = new BlueShiftDSP.FeedBackCombFilter();

    private void Awake()
    {
        feedBackCombFilterl.SetSampleRate(AudioSettings.outputSampleRate);
        feedBackCombFilterr.SetSampleRate(AudioSettings.outputSampleRate);
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
                data[n] = gainL * feedBackCombFilterl.Filter(data[n], delay);
            else
                data[n] = gainR * feedBackCombFilterr.Filter(data[n], delay);

            n++;
        }

    }
}
