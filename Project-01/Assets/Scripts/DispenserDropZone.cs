using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class DispenserDropZone : UdonSharpBehaviour
{
    public TicketDispenser dispenser;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DropZone] Trigger entered by: {other.gameObject.name}");

        if (other == null) { Debug.Log("[DropZone] other null"); return; }
        if (dispenser == null) { Debug.Log("[DropZone] dispenser null"); return; }

        Ticket ticket = other.GetComponentInParent<Ticket>();
        if (ticket == null)
        {
            Debug.Log($"[DropZone] {other.gameObject.name} is not a ticket");
            return;
        }

        VRC_Pickup pk = (VRC_Pickup)ticket.GetComponent(typeof(VRC_Pickup));
        if (pk != null && pk.IsHeld)
        {
            Debug.Log("[DropZone] Ticket still held — waiting for drop");
            return;
        }

        Debug.Log($"[DropZone] Returning ticket {ticket.ticketNumber}");
        dispenser.ReturnTicket(ticket.ticketNumber);
    }
}