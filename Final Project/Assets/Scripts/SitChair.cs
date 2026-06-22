using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SitChair : UdonSharpBehaviour
{
    public VRCStation station;

    public override void Interact()
    {
        station.UseStation(Networking.LocalPlayer);
    }
}