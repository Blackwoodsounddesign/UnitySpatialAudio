using UnityEngine;


/// <summary>
/// The spatial audio listener goes onto the camera that you wish to percieve spatial audio.
/// 
/// There can only be one spatial audio listener in a scene.
/// </summary>
[RequireComponent(typeof(AudioListener))]
public class SpatialAudioListener : MonoBehaviour
{
    public AudioListener audioListener;

    public Vector3 ListenerPlacement { get; private set; }
    public Vector3 ListenerView { get; private set; }
    public Vector3 ListenerZAxis { get; private set; }

    private static SpatialAudioListener spatialAudioListener;
    public static SpatialAudioListener SpatialListener { get { return spatialAudioListener; } }

    /// <summary>
    /// On awake, we make sure that there is only one Audio Listener in the scene,
    /// and then grab the audio listener on this object.  
    /// </summary>
    private void Awake()
    {
        if (spatialAudioListener != null && spatialAudioListener != this)
            Destroy(this.gameObject);
        else
            spatialAudioListener = this;

        audioListener = (AudioListener)this.GetComponent("AudioListener");
    }

    /// <summary>
    /// We update the placement 
    /// </summary>
    void Update()
    {
        ListenerPlacement = audioListener.transform.position;
        ListenerView = audioListener.transform.rotation * Vector3.forward;
        ListenerZAxis = audioListener.transform.up;
    }
}