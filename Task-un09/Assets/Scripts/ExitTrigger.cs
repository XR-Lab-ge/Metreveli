using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ExitTrigger : UdonSharpBehaviour
{
    public TicketDispenser dispenser;
    public DoorController door;

    [Header("Time to wait before closing (seconds)")]
    public float closeDelay = 5f;  // 5 წამი

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == null || !player.isLocal) return;
        if (door == null) return;

        Debug.Log($"[Exit] Player crossed exit — closing in {closeDelay}s.");
        SendCustomEventDelayedSeconds(nameof(CloseDoor), closeDelay);
    }

    public void CloseDoor()
    {
        if (door != null) door.CloseDoor();
    }
}