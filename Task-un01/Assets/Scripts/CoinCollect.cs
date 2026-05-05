using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class CoinCollect : UdonSharpBehaviour
{
    // Player-ი შევიდა Trigger ზონაში
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        // მხოლოდ local player-ისთვის
        if (player.isLocal)
        {
            // Coin ქრება
            gameObject.SetActive(false);
        }
    }
}