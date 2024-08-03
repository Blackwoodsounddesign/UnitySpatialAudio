using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioFXToolkitDSP;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Direction))]
public class Obfuscation : MonoBehaviour
{
    public float obfuscatedFrequency = 5000;

    private GameObject audiosource;
    int layerMask;

    //Listener data
    private SpatialAudioListener Listener;
    private Direction direction;

    [SerializeField] bool obfuscated = false;
    [SerializeField] string[] LayersToIgnore = new string[] { "Ignore Raycast", "Player" };

    //
    private int sample_rate;
    SimpleFilter[] simpleFilters; 

    private void OnValidate()
    {
        audiosource = gameObject;
    }

    public Obfuscation()
    {
        simpleFilters = new SimpleFilter[4]; 

        for (int i = 0; i < 4; i++)
            simpleFilters[i] = new SimpleFilter();
    }

    private void Awake()
    {
        sample_rate = AudioSettings.outputSampleRate;

        simpleFilters[0].SetFilterParameters(2, sample_rate);
        simpleFilters[1].SetFilterParameters(2, sample_rate);
        simpleFilters[2].SetFilterParameters(20000, sample_rate);
        simpleFilters[3].SetFilterParameters(20000, sample_rate);
    }

    void Start()
    {
        Listener = SpatialAudioListener.SpatialListener;
        direction = GetComponent<Direction>();

        foreach (string Layer in LayersToIgnore)
        {
            layerMask += 1 << LayerMask.NameToLayer(Layer);
        }
        layerMask = ~layerMask;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        bool ray = Physics.Raycast(transform.position, direction.GetDirection(), out hit, direction.GetDistance(), layerMask);
        if (ray)
        {
            obfuscated = true;
        }
        else
        {
            obfuscated = false;
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //makes sure the audio is stereo
        if (channels != 2)
            return;

        float filterFQ = 20000;

        if (obfuscated)
        {
            filterFQ = obfuscatedFrequency;
        }

        int dataLen = data.Length;

        int n = 0;
        //process block, this is interleaved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (channeliter == 0)
            {
                float control_freq = simpleFilters[0].Filter(filterFQ);
                simpleFilters[2].SetFilterParameters(control_freq, sample_rate);
                data[n] = simpleFilters[2].Filter(data[n]);
            }
            else
            {
                float control_freq = simpleFilters[1].Filter(filterFQ);
                simpleFilters[3].SetFilterParameters(control_freq, sample_rate);
                data[n] = simpleFilters[3].Filter(data[n]);
            }

            n++;
        }

    }
}
