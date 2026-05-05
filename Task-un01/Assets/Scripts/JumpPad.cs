using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class JumpPad : UdonSharpBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10f;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            // Player-ის მიმდინარე velocity
            Vector3 vel = player.GetVelocity();

            // Y velocity გაანულე, ზემოთ force დაამატე
            vel.y = jumpForce;
            player.SetVelocity(vel);

            Debug.Log("JumpPad! Force: " + jumpForce);
        }
    }
}