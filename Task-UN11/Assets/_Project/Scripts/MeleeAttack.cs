using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public Camera fpsCam;
    public float damage = 60f;
    public float range = 2.5f;
    public float cooldown = 0.6f;
    public AudioClip swingSfx;

    float nextAttack;
    AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
        if (!fpsCam) fpsCam = Camera.main;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextAttack)
        {
            nextAttack = Time.time + cooldown;
            Strike();
        }
    }

    void Strike()
    {
        if (swingSfx) audioSrc.PlayOneShot(swingSfx, 0.5f);
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }
}