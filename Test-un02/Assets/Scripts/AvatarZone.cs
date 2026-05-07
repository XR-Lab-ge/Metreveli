using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class AvatarZone : UdonSharpBehaviour
{
    public GameObject highlightEffect;
    public GameObject applyButton;

    void Start()
    {
        if (highlightEffect != null) highlightEffect.SetActive(false);
        if (applyButton != null) applyButton.SetActive(false);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        if (highlightEffect != null) highlightEffect.SetActive(true);
        if (applyButton != null) applyButton.SetActive(true);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        if (highlightEffect != null) highlightEffect.SetActive(false);
        if (applyButton != null) applyButton.SetActive(false);
    }
}