using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ChairSit : UdonSharpBehaviour
{
    private VRCStation station;
    private bool isSeated = false;

    void Start()
    {
        station = GetComponent<VRCStation>();
    }

    // კლიკი - ჯდება ან დგება
    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null) return;

        if (!isSeated)
        {
            station.UseStation(player);
        }
        else
        {
            station.ExitStation(player);
        }
    }

    public override void OnStationEntered(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            isSeated = true;
        }
    }

    public override void OnStationExited(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            isSeated = false;
        }
    }
}