using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

public class ApplyButton : UdonSharpBehaviour
{
    public VRCAvatarPedestal pedestal;

    public override void Interact()
    {
        if (pedestal == null) return;
        pedestal.SetAvatarUse(Networking.LocalPlayer);
    }
}