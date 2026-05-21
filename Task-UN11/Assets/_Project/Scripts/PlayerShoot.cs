using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera fpsCamera;
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.15f;
    public AudioClip shootSound;
    public GameObject muzzleFlashPrefab;
    public Transform firePoint;
    public LineRenderer tracerPrefab;

    float nextFire = 0;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        if (!fpsCamera) fpsCamera = Camera.main;
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (Input.GetMouseButton(0) && Time.time >= nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (shootSound) audioSource.PlayOneShot(shootSound, 0.4f);
        if (muzzleFlashPrefab && firePoint)
        {
            GameObject fx = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(fx, 0.1f);
        }
        Vector3 origin = fpsCamera.transform.position;
        Vector3 dir = fpsCamera.transform.forward;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, range))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }
}