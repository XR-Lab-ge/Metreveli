using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class TouchScript : UdonSharpBehaviour
{
    public Color touchColor = new Color(0.2f, 0.6f, 1.0f);
    public Color defaultColor = new Color(0.55f, 0.37f, 0.24f);

    private Renderer[] renderers;
    private bool isColored = false;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        SetColor(defaultColor);
        DisableInteractive = true;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) DisableInteractive = false;
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal) DisableInteractive = true;
        // ფერი აღარ ბრუნდება — სამუდამოდ რჩება ✅
    }

    public override void Interact()
    {
        isColored = !isColored;
        SetColor(isColored ? touchColor : defaultColor);
    }

    private void SetColor(Color c)
    {
        foreach (Renderer r in renderers)
            if (r != null) r.material.color = c;
    }
}