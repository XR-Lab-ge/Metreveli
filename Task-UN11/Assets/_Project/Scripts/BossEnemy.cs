using UnityEngine;

public class BossEnemy : EnemyAI
{
    [Header("Boss")]
    public string bossName = "OVERLORD-01";

    protected override void Start()
    {
        maxHealth = 500f;
        damage = 20f;
        attackRange = 3f;
        attackCooldown = 1.5f;
        scoreValue = 1000;
        base.Start();

        transform.localScale *= 1.8f;

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterBoss(this);
    }
}