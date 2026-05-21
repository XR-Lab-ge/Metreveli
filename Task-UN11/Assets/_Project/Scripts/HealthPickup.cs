using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 30f;
    public float rotateSpeed = 90f;
    public float bobAmplitude = 0.2f;
    public float bobSpeed = 2f;

    Vector3 startPos;
    bool used = false;

    void Start()
    {
        startPos = transform.position;
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;
        var hp = other.GetComponent<PlayerHealth>();
        if (hp == null) return;
        if (hp.GetHealthPercent() >= 0.999f) return;
        hp.Heal(healAmount);
        used = true;
        Destroy(gameObject);
    }
}