using UnityEngine;

public class StartWaveTrigger : MonoBehaviour
{
    public float interactRange = 3f;
    Transform player;
    float logCooldown = 0;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;
        Debug.Log($"[StartWaveTrigger] Start. Player found: {(player != null ? "YES" : "NO")}");
    }

    void Update()
    {
        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist < interactRange * 2)
        {
            logCooldown -= Time.deltaTime;
            if (logCooldown <= 0)
            {
                logCooldown = 1f;
                Debug.Log($"[StartWaveTrigger] Player near. Distance={dist:F2}, range={interactRange}, state={GameManager.Instance?.state}");
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[StartWaveTrigger] E PRESSED! Distance={dist:F2}, in range={dist < interactRange}, state={GameManager.Instance?.state}");

            if (dist < interactRange)
            {
                GameManager.Instance?.OnWaveStarted();
            }
            else
            {
                Debug.Log($"[StartWaveTrigger] TOO FAR. Need distance < {interactRange}, got {dist:F2}");
            }
        }
    }
}