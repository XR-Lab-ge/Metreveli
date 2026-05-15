using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

public class Ticket : UdonSharpBehaviour
{
    [Tooltip("0..4 — შესაბამისი სკამის ნომერი")]
    public int ticketNumber;

    [Tooltip("ყველა სკამი წინასწარ მიანიჭე — index შეესაბამება ticketNumber-ს")] // არ დაგავიწყდეს დემე
    public ChairButton[] chairs;

    public override void OnPickupUseDown()
    {
        VRCPlayerApi local = Networking.LocalPlayer;
        if (local == null) return;

        if (chairs == null || ticketNumber >= chairs.Length || chairs[ticketNumber] == null)
        {
            Debug.Log("[Ticket] No chair assigned.");
            return;
        }

        ChairButton myChair = chairs[ticketNumber];

        float distance = Vector3.Distance(local.GetPosition(), myChair.transform.position);
        if (distance > 2.5f)
        {
            Debug.Log($"[Ticket] Too far from Chair {ticketNumber + 1} ({distance:F1}m)");
            return;
        }

        myChair.station.UseStation(local);
        Debug.Log($"[Ticket] Sat on Chair {ticketNumber + 1}");
    }
}