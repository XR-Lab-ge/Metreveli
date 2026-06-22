using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class MovementController : UdonSharpBehaviour
{
    void Start()
    {
        LockMovement();
        SendCustomEventDelayedSeconds(nameof(LockMovement), 0.5f);
        SendCustomEventDelayedSeconds(nameof(LockMovement), 2f);
        SendCustomEventDelayedSeconds(nameof(LockMovement), 5f);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player != null && player.isLocal)
            LockMovement();
    }

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player != null && player.isLocal)
            LockMovement();
    }

    public void LockMovement()
    {
        VRCPlayerApi p = Networking.LocalPlayer;
        if (p == null) return;
        p.SetWalkSpeed(0f);
        p.SetRunSpeed(0f);
        p.SetStrafeSpeed(0f);
    }
}
