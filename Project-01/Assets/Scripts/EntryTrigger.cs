using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class EntryTrigger : UdonSharpBehaviour
{
    public TicketDispenser dispenser;
    private bool used = false;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == null || !player.isLocal) return;
        if (used) return;

        Vector3 vel = player.GetVelocity();
        if (vel.z <= 0.1f)
        {
            Debug.Log($"[Entry] Player not entering (vz={vel.z:F2}) — skipping.");
            return;
        }

        if (PlayerHasTicket(player))
        {
            Debug.Log("[Entry] Player already has ticket — skipping.");
            return;
        }

        used = true;
        dispenser.IssueTicket();
        SendCustomEventDelayedSeconds(nameof(ResetTrigger), 3f);
    }

    public void ResetTrigger() { used = false; }

    private bool PlayerHasTicket(VRCPlayerApi player)
    {
        if (dispenser == null) return false;
        foreach (GameObject g in dispenser.tickets)
        {
            if (g == null || !g.activeSelf) continue;
            VRC_Pickup p = (VRC_Pickup)g.GetComponent(typeof(VRC_Pickup));
            if (p != null && p.IsHeld && p.currentPlayer == player) return true;
        }
        return false;
    }
}