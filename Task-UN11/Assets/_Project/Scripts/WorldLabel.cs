using UnityEngine;
using TMPro;

[ExecuteAlways]
public class WorldLabel : MonoBehaviour
{
    public string label = "INTERACT [E]";
    public Color color = Color.red;
    public Vector3 offset = new Vector3(0, 2, 0);
    public float fontSize = 4;
    public float bobAmplitude = 0.1f;
    public float bobSpeed = 2;

    private TextMeshPro tmp;

    void OnEnable()
    {
        SetupLabel();
    }

    void SetupLabel()
    {
        Transform existing = transform.Find("__WorldLabel");
        GameObject labelGO;

        if (existing != null) labelGO = existing.gameObject;
        else
        {
            labelGO = new GameObject("__WorldLabel");
            labelGO.transform.SetParent(transform);
        }

        labelGO.transform.localPosition = offset;

        tmp = labelGO.GetComponent<TextMeshPro>();
        if (tmp == null) tmp = labelGO.AddComponent<TextMeshPro>();

        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        if (tmp.fontMaterial != null) tmp.fontMaterial.SetColor("_FaceColor", color);
    }

    void Update()
    {
        if (tmp == null) { SetupLabel(); return; }

        if (Application.isPlaying)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            tmp.transform.position = transform.position + offset + Vector3.up * bob;
        }
        else
        {
            tmp.transform.localPosition = offset;
        }

        if (Camera.main != null)
            tmp.transform.forward = Camera.main.transform.forward;

        if (tmp.color != color)
        {
            tmp.color = color;
            if (tmp.fontMaterial != null) tmp.fontMaterial.SetColor("_FaceColor", color);
        }
    }
}