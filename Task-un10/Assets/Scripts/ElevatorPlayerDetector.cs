using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ElevatorPlayerDetector : UdonSharpBehaviour
{
    public ElevatorController controller;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player == null || !player.isLocal) return;
        if (controller != null) controller.OnPassengerEntered();
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player == null || !player.isLocal) return;
        if (controller != null) controller.OnPassengerExited();
    }
}