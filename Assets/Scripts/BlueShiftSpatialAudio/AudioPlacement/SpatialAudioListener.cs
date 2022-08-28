using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class SpatialAudioListener : MonoBehaviour
{
    private static SpatialAudioListener spatialAudioListener;
    public static SpatialAudioListener SpatialListener { get { return spatialAudioListener; } }

    private void Awake()
    {
        if (spatialAudioListener != null && spatialAudioListener != this)
            Destroy(this.gameObject);
        else
            spatialAudioListener = this;
    }

    public AudioListener audioListener;

    public Vector3 ListenerPlacement { get; private set; }
    public Vector3 ListenerView { get; private set; }
    public Vector3 ListenerZAxis { get; private set; }

    void Start()
    {
        audioListener = FindObjectOfType<AudioListener>();
    }

    void Update()
    {
        ListenerPlacement = audioListener.transform.position;
        ListenerView = audioListener.transform.rotation * Vector3.forward;
        ListenerZAxis = audioListener.transform.up;
    }
}