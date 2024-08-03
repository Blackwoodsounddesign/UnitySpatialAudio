using UnityEngine;
using AudioFXToolkitDSP;
using System;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Direction))]
public class SpatialAudioObject : MonoBehaviour
{
    [Header("Spatial Settings")]
    [Range(0,3), SerializeField, Tooltip("0 is almost no spatial effect, 3 is max effect.")]
    private float SpatialScale = 1; 

    [Header("Distance Attenuation Settings")]
    [Range(1, 30), SerializeField, Tooltip("This minimum distance at which the audio lowers in volume as the listener moves further away.")]
    private float minimumDistance = 1;
    [Range(0.01f, 1), SerializeField, Tooltip("The scale fade curve over distance. 0 is a long and 1 is short.")]
    private float attenuationScale = 0.05f;

    public enum VolumeRollOff
    {
        Inverse, 
        Logorithmic,
        Linear
    };

    [SerializeField, Tooltip("The type of fade curve.")]
    private VolumeRollOff volumeRollOff;

    SpatialFilter[] spatialFilters;
    DelayLine[] delayLines;
    SimpleFilter[] simpleFilters;
    HighShelfFilter[] highShelfFilters;

    [SerializeField, HideInInspector]
    Direction direction;
    [SerializeField, HideInInspector]
    AudioSource audioSource;

    private float m_azimuth = 0;
    private float m_elevation;
    private float m_distance = 1;

    private float ITD_delayLeft = 0;
    private float ITD_delayRight = 0;
    private float gainreductionL = 1;
    private float gainreductionR = 1;
    private float filterGainIncrease = 0;
    private float filterGainDecrease = 0;

    private int sample_rate;

    /// <summary>
    /// When creating the spatial audio object we leverage low cost DSP.  
    /// </summary>
    public SpatialAudioObject()
    {
        spatialFilters = new SpatialFilter[2];
        delayLines = new DelayLine[2];
        simpleFilters = new SimpleFilter[6];
        highShelfFilters = new HighShelfFilter[2];

        for (int i = 0; i < 2; i++)
        {
            delayLines[i] = new DelayLine(32768); 
            spatialFilters[i] = new SpatialFilter();
            highShelfFilters[i] = new HighShelfFilter();
        }

        for (int i = 0; i < 6; i++)
            simpleFilters[i] = new SimpleFilter();
    }

    /// <summary>
    /// Helpful Inspector GUI
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Display the minimumDistance radius when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, minimumDistance);
    }

    /// <summary>
    /// Gets the direction and audio source automatically when placing the script.
    /// </summary>
    private void OnValidate()
    {
        direction = GetComponent<Direction>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Gets the sample rate and sets some base case filter parameters. 
    /// </summary>
    private void Awake()
    {
        sample_rate = AudioSettings.outputSampleRate;

        for (int i = 0; i < 2; i++)
        {
            delayLines[i].SetSampleRate(sample_rate);
            spatialFilters[i].SetFilterParameters(sample_rate, 500);
            highShelfFilters[i].SetFilterParameters(sample_rate, 2000);
        }
        for (int i = 0; i < 6; i++)
            simpleFilters[i].SetFilterParameters(2, sample_rate);

        audioSource.spatialBlend = 0;
    }

    /// <summary>
    /// Per frame, update the rolloff and the spatial filter parameters. 
    /// </summary>
    private void Update()
    {
        //these set control parameters
        DistanceBasedAmplitudeRollOff();
        m_azimuth = direction.GetAzimuth();
        SetSpatialParameters();
    }

    private void DistanceBasedAmplitudeRollOff()
    {
        float distance = direction.GetDistance();

        if (distance > minimumDistance)
        {
            switch (volumeRollOff)
            {
                case VolumeRollOff.Logorithmic:
                    m_distance = distance - minimumDistance + 1;
                    m_distance = (float)((-1 * Mathf.Log(m_distance, 20 / attenuationScale)) + 1);

                    if (m_distance < 0)
                        m_distance = 0;
                    break;

                case VolumeRollOff.Inverse:
                    m_distance = distance - minimumDistance + 1;
                    m_distance = 1 / m_distance * (1 - attenuationScale);
                    break;

                case VolumeRollOff.Linear:
                    m_distance = distance - minimumDistance + 1;
                    m_distance = (-1 * m_distance * attenuationScale) + 1;

                    if (m_distance < 0)
                        m_distance = 0;
                    break;
            }
        }

        if (distance < minimumDistance && m_distance != 1)
        {
            //makes sure to set this back to 1 when inside of the minimum distance radius
            if (m_distance > 1)
            {
                m_distance = Mathf.Clamp(m_distance - 0.05f, 1f, 1.6f);
            }
            else if (m_distance < 1)
            {
                m_distance = Mathf.Clamp(m_distance + 0.05f, 1f, 1.6f);
            }
        }
    }

    void SetSpatialParameters()
    {
        SetForwardBackSpatialFilters();
        
        if (m_azimuth > 0)
        {
            ITD_delayLeft = m_azimuth / 180;

            if (ITD_delayLeft > 0.5f)
            {
                float tempL = ITD_delayLeft - 0.5f;
                ITD_delayLeft = 0.5f - tempL;
            }

            gainreductionL = 1 - (ITD_delayLeft * 1.2f);
            ITD_delayLeft *= 2f * SpatialScale;

            spatialFilters[0].SetFilterParameters(sample_rate, 290f, -5f * ITD_delayLeft, 0.0707f);
            spatialFilters[1].SetFilterParameters(sample_rate, 4500, 2f * ITD_delayLeft, 0.2f);

            //quickly brings the opposite delay to zero
            if (ITD_delayRight > 0)
                ITD_delayRight = Mathf.Clamp(ITD_delayRight - 0.05f, 0f, 0.5f);
        }
        else
        {
            ITD_delayRight = Mathf.Abs(m_azimuth / 180);

            if (ITD_delayRight > 0.5f)
            {
                float tempR = ITD_delayRight - 0.5f;
                ITD_delayRight = 0.5f - tempR;
            }

            gainreductionR = 1 - (ITD_delayRight * 1.2f);
            ITD_delayRight *= 2f * SpatialScale;

            spatialFilters[1].SetFilterParameters(sample_rate, 290f, -5f * ITD_delayRight, 0.0707f);
            spatialFilters[0].SetFilterParameters(sample_rate, 4500, 2f * ITD_delayRight, 0.2f);

            //quickly brings the opposite delay to zero
            if (ITD_delayLeft > 0)
                ITD_delayLeft = Mathf.Clamp(ITD_delayLeft - 0.05f, 0f, 0.5f);
        }

    }

    private void SetForwardBackSpatialFilters()
    {
        float m_FowardBackFilterControl = Mathf.Abs(m_azimuth / 180);

        if (m_FowardBackFilterControl < 0.5)
        {
            filterGainIncrease = 1f - (m_FowardBackFilterControl * 2f);
            highShelfFilters[0].SetFilterParameters(sample_rate, 2000, filterGainIncrease * 2f * SpatialScale);
            highShelfFilters[1].SetFilterParameters(sample_rate, 2000, filterGainIncrease * 2f * SpatialScale);
        }
        else
        {
            filterGainDecrease = (m_FowardBackFilterControl - 0.5f) * -2f;
            highShelfFilters[0].SetFilterParameters(sample_rate, 2000, filterGainDecrease * 2f * SpatialScale);
            highShelfFilters[1].SetFilterParameters(sample_rate, 2000, filterGainDecrease * 2f * SpatialScale);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //makes sure the audio is stereo
        if (channels != 2)
            return;

        int dataLen = data.Length;

        int n = 0;
        //process block, this is interleaved
        while (n < dataLen)
        {
            //pull out the left and right channels 
            int channeliter = n % channels;

            if (channeliter == 0)
            {
                //hypergeneralized binaural filters
                data[n] = spatialFilters[0].Filter(data[n]);
                data[n] = highShelfFilters[0].Filter(data[n]);

                //Time and level differences 
                delayLines[0].WriteDelay(data[n]);
                data[n] = delayLines[0].DelayTap(simpleFilters[0].Filter(ITD_delayLeft)) * simpleFilters[2].Filter(gainreductionL);
                data[n] *= simpleFilters[4].Filter(m_distance);
            }
            else
            {
                //hypergeneralized binaural filters
                data[n] = spatialFilters[1].Filter(data[n]);
                data[n] = highShelfFilters[1].Filter(data[n]);

                //Time and level differences
                delayLines[1].WriteDelay(data[n]);
                data[n] = delayLines[1].DelayTap(simpleFilters[1].Filter(ITD_delayRight)) * simpleFilters[3].Filter(gainreductionR);
                data[n] *= simpleFilters[5].Filter(m_distance);
            }

            n++;
        }

    }
}
