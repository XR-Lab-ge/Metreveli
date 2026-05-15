using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ElevatorButton : UdonSharpBehaviour
{
    public ElevatorController controller;
    public int targetFloor = 0;

    public override void Interact()
    {
        if (controller == null) return;
        controller.RequestFloor(targetFloor);
    }
}