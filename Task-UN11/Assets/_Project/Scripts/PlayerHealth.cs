using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public Image healthBarFill;
    public AudioClip hurtSfx;
    public AudioClip healSfx;

    float currentHealth;
    AudioSource audioSrc;
    bool dead;

    void Start()
    {
        currentHealth = maxHealth;
        audioSrc = GetComponent<AudioSource>();
        if (!audioSrc) audioSrc = gameObject.AddComponent<AudioSource>();
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (dead) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (hurtSfx) audioSrc.PlayOneShot(hurtSfx, 0.6f);
        UpdateUI();
        if (currentHealth <= 0)
        {
            dead = true;
            if (GameManager.Instance) GameManager.Instance.OnPlayerDied();
        }
    }

    public void Heal(float amount)
    {
        if (dead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (healSfx) audioSrc.PlayOneShot(healSfx, 0.6f);
        UpdateUI();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        dead = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthBarFill) healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    public float GetHealthPercent() => currentHealth / maxHealth;
}