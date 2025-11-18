using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    private Animator anim;
    private bool isDead = false;
    public bool isBlocking = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (isBlocking)
        {
            Debug.Log("Attack blocked!");
            return;
        }

        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player healed {amount}. Current health: {currentHealth}");
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player died.");
        anim.SetTrigger("IsDead");
        GameManager.Instance.PlayerDied(); // Notify GameManager
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log("Player health reset to max.");
    }

}
