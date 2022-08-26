using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Direction : MonoBehaviour
{
    public GameObject audiosource;

    [SerializeField] private float HorizontalAngle;
    public float GetAzimuth() => HorizontalAngle;

    [SerializeField] private float VerticalAngle;
    public float GetElevation() => VerticalAngle;

    [SerializeField] private float distance;
    public float GetDistance() => distance;

    //containers for the object and listener. 
    private Vector3 objectplacement;
    public Vector3 GetObjectPlacement() => objectplacement;

    //Listener data
    private SpatialAudioListener Listener;
    private Vector3 listenerplacement;

    private void Start()
    {
        //audioListener = FindObjectOfType<AudioListener>();
        audiosource = this.gameObject;
        Listener = SpatialAudioListener.SpatialListener;
    }

    private void Update()
    {
        /**
         * Create the object and listener vector. These two are compared to find the location of audio sources during runtime.
         */
        objectplacement = audiosource.transform.position - Listener.ListenerPlacement;
        listenerplacement = Listener.ListenerView;

        ///////These are two helpful debug lines./////
        //
        //This is "listener" vector
        //Debug.DrawLine(audioListener.transform.position, audioListener.transform.position + audioListener.transform.forward, Color.green);
        //
        //this is the "object" vector 
        //Debug.DrawLine(audioListener.transform.position, audioListener.transform.position + objectplacement, Color.blue);
        //
        /////End//////

        if (listenerplacement.x != 0 && listenerplacement.z != 0)
        {
            Vector3 listenerplacementH = new Vector3(listenerplacement.x, 0, listenerplacement.z);
            Vector3 objectplacementH = new Vector3(objectplacement.x, 0, objectplacement.z);

            HorizontalAngle = Vector3.SignedAngle(listenerplacementH, objectplacementH, Listener.ListenerZAxis);
        }

        /**
         * This finds the vertical angle to the audio source. This is done by only comparing the y compenents of the two vectors. 
         */

        Vector3 listenerplacementV = new Vector3(objectplacement.normalized.x, listenerplacement.y, objectplacement.normalized.z);
        Vector3 objectplacementV = new Vector3(objectplacement.normalized.x, objectplacement.normalized.y, objectplacement.normalized.z);

        VerticalAngle = -1 * Vector3.SignedAngle(listenerplacementV, objectplacementV, Vector3.forward);

        /**
         * This is the distnace from the listener to the audiosource by finding the magintude of the vector. 
         */
        distance = objectplacement.magnitude;

    }
}