using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class SitButtonScript : UdonSharpBehaviour
{
    public ChairSit chairSit;

    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null) return;

        if (!chairSit.isSeated)
        {
            chairSit.SitDown();
        }
        else
        {
            chairSit.StandUp();
        }
    }
}