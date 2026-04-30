using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class TouchScript : UdonSharpBehaviour
{
    public Color touchColor = new Color(0.2f, 0.6f, 1.0f); // ლურჯი
    public Color defaultColor = new Color(0.55f, 0.37f, 0.24f); // ყავისფერი

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
        if (player.isLocal) DisableInteractive = false; // ზონაში შესვლისას click ირთვება
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            DisableInteractive = true; // გამოსვლისას ითიშება
            isColored = false;
            SetColor(defaultColor); // ფერიც ბრუნდება დეფოლტზე
        }
    }

    public override void Interact()
    {
        isColored = !isColored; // Toggle ლოგიკა
        SetColor(isColored ? touchColor : defaultColor);
    }

    private void SetColor(Color c)
    {
        foreach (Renderer r in renderers)
        {
            if (r != null) r.material.color = c;
        }
    }
}