using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ChairSit : UdonSharpBehaviour
{
    private VRCStation station;

    public bool isSeated = false;

    void Start()
    {
        station = GetComponent<VRCStation>();
    }

    public void SitDown()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null) return;
        if (!isSeated) station.UseStation(player);
    }

    public void StandUp()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null) return;
        if (isSeated) station.ExitStation(player);
    }

    public override void OnStationEntered(VRCPlayerApi player)
    {
        if (player.isLocal) isSeated = true;
    }

    public override void OnStationExited(VRCPlayerApi player)
    {
        if (player.isLocal) isSeated = false;
    }
}