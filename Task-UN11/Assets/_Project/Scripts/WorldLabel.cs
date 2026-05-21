using UnityEngine;
using TMPro;

public class WorldLabel : MonoBehaviour
{
    public string label = "PICKUP";
    public Color color = new Color(0f, 0.9f, 1f, 1f);
    public Vector3 offset = new Vector3(0, 2f, 0);
    public float fontSize = 4f;
    public float bobAmplitude = 0.1f;
    public float bobSpeed = 2f;

    TextMeshPro tmp;
    Camera cam;
    Vector3 baseLocal;

    void Start()
    {
        cam = Camera.main;
        GameObject child = new GameObject("Label_TMP");
        child.transform.SetParent(transform, false);
        child.transform.localPosition = offset;
        baseLocal = child.transform.localPosition;
        tmp = child.AddComponent<TextMeshPro>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = Color.black;
    }

    void LateUpdate()
    {
        if (!tmp) return;
        if (!cam) cam = Camera.main;
        if (cam) tmp.transform.rotation = Quaternion.LookRotation(tmp.transform.position - cam.transform.position);
        tmp.transform.localPosition = baseLocal + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
    }
}