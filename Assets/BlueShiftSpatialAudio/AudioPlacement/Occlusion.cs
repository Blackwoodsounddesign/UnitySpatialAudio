using UnityEngine;
using AudioFXToolkitDSP;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Direction))]
public class Occlusion : MonoBehaviour
{
    [SerializeField] private float obfuscatedFrequency = 5000;
    [SerializeField] private float dBReduction = -10f;

    private GameObject audiosource;
    int layerMask;
    [SerializeField] string[] LayersToIgnore = new string[] { "Ignore Raycast", "Player" };

    //Listener data
    private SpatialAudioListener Listener;
    private Direction direction;

    //DSP variables
    private int sample_rate;
    private bool occluded = false;
    private float occludedPercentage = 0f;
    SimpleFilter[] simpleFilters;
    HighShelfFilter[] highShelfFilters;

    private void OnValidate()
    {
        audiosource = gameObject;
    }

    public Occlusion()
    {
        simpleFilters = new SimpleFilter[2];
        highShelfFilters = new HighShelfFilter[2];

        for (int i = 0; i < 2; i++)
        {
            simpleFilters[i] = new SimpleFilter();
            highShelfFilters[i] = new HighShelfFilter();
        }

    }

    private void Awake()
    {
        sample_rate = AudioSettings.outputSampleRate;

        simpleFilters[0].SetFilterParameters(0.5f, sample_rate);
        simpleFilters[1].SetFilterParameters(0.5f, sample_rate);
        highShelfFilters[0].SetFilterParameters(sample_rate, obfuscatedFrequency);
        highShelfFilters[1].SetFilterParameters(sample_rate, obfuscatedFrequency);
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
    void Update()
    {
        RaycastHit hit;

        Vector3[] directions = new Vector3[]
        {
            direction.GetDirection(),
            direction.GetDirection() + Vector3.up * 0.1f,
            direction.GetDirection() - Vector3.up * 0.1f,
            direction.GetDirection() + Vector3.right * 0.1f,
            direction.GetDirection() - Vector3.right * 0.1f
        };

        float distance = direction.GetDistance();

        occludedPercentage = 0f;
        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, out hit, distance, layerMask))
            {
                occludedPercentage += 0.2f; 
            }
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //makes sure the audio is stereo
        if (channels != 2)
            return;

        int dataLen = data.Length;

        float appliedDBReduction = dBReduction * occludedPercentage;

        int n = 0;
        //process block, this is interleaved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            //use the channel iterator to select from the filterbanks
            float control_freq = simpleFilters[channeliter].Filter(appliedDBReduction);
            highShelfFilters[channeliter].SetFilterParameters(sample_rate, obfuscatedFrequency, control_freq);
            data[n] = highShelfFilters[channeliter].Filter(data[n]);
            n++;
            
        }

    }
}
