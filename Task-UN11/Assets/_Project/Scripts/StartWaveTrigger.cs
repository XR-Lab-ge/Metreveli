using UnityEngine;

public class StartWaveTrigger : MonoBehaviour
{
    public float interactRange = 3f;
    Transform player;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;
        if (Vector3.Distance(player.position, transform.position) < interactRange &&
            Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance?.OnWaveStarted();
        }
    }
}