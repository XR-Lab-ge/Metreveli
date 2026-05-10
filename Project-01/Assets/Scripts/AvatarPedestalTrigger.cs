using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

public class AvatarPedestalTrigger : UdonSharpBehaviour
{
    public TicketDispenser dispenser;
    public VRCAvatarPedestal pedestal;

    public override void Interact()
    {
        if (pedestal != null)
            pedestal.SetAvatarUse(Networking.LocalPlayer);

        if (dispenser != null)
            dispenser.RequestEntry();
    }
}