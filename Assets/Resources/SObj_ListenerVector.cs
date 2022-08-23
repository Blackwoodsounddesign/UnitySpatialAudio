using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ListenerVector", menuName = "BlueShiftSpatialAudio/Listener", order = 1)]
public class SObj_ListenerVector : ScriptableObject
{
    public Vector3 ListenerPlacement;
    public Vector3 ListenerView;
    public Vector3 ListenerZAxis;
}
