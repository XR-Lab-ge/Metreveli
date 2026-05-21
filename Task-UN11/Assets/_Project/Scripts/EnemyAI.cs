using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float damage = 12f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.2f;
    public int scoreValue = 100;

    [Header("FX")]
    public Color hitFlashColor = Color.red;
    public float hitFlashDuration = 0.08f;

    protected float currentHealth;
    protected float lastAttackTime;
    protected NavMeshAgent agent;
    protected Transform player;
    protected Renderer[] renderers;
    protected Color[] originalColors;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        GameObject p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
    }

    protected virtual void Update()
    {
        if (player == null || currentHealth <= 0) return;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph) ph.TakeDamage(damage);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        StopAllCoroutines();
        StartCoroutine(HitFlash());
        if (currentHealth <= 0) Die();
    }

    System.Collections.IEnumerator HitFlash()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] && renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] && renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
    }

    protected virtual void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyKilled(scoreValue);
        Destroy(gameObject);
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}