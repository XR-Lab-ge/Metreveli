using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public float rotateSpeed = 60f;
    public float bobAmplitude = 0.15f;
    public float bobSpeed = 2f;
    public float interactRange = 3f;

    Vector3 startPos;
    Transform player;

    void Start()
    {
        startPos = transform.position;
        var p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;

        if (player == null) return;
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < interactRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[WeaponPickup] E pressed. Distance={dist:F2}. GameManager state={GameManager.Instance?.state}");
            GameManager.Instance?.OnWeaponPickedUp();
        }
    }
}