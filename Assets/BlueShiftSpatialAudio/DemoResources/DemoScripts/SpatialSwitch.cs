using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpatialSwitch : MonoBehaviour
{
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SpatialAudioObject spatialAudioObject;
    [SerializeField] private Occlusion occlusion;
    public Text text;


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            if (audioSource.spatialBlend == 0)
            {
                audioSource.spatialBlend = 1;
                spatialAudioObject.enabled = false;
                occlusion.enabled = false;
                text.text = "Unity System On";
            }
            else
            {
                spatialAudioObject.enabled = true;
                occlusion.enabled = true;
                audioSource.spatialBlend = 0;
                text.text = "Spatial System On";
            }
        }
    }
}
