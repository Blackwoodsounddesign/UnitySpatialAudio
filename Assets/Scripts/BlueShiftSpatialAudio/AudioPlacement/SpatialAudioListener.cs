using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialAudioListener : MonoBehaviour
{
    private AudioListener audioListener;
    private SObj_ListenerVector sObj_ListenerVector;

    void Start()
    {
        audioListener = FindObjectOfType<AudioListener>();
        sObj_ListenerVector = Resources.Load("ListenerVector") as SObj_ListenerVector;
    }

    void Update()
    {
        sObj_ListenerVector.ListenerPlacement = audioListener.transform.position;
        sObj_ListenerVector.ListenerView = audioListener.transform.rotation * Vector3.forward;
        sObj_ListenerVector.ListenerZAxis = audioListener.transform.up;
    }
}