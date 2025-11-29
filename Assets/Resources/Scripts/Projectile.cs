using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType = ProjectileType.Player;
    [SerializeField, Range(1, 10)] private float lifetime = 0.5f;
    [SerializeField] private float damage = 10f;

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool hasImpacted = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(LifetimeCoroutine());
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TriggerImpact();

        if (projectileType == ProjectileType.Player)
        {
            // Damage enemy
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
        }
        else if (projectileType == ProjectileType.Enemy)
        {
            // Damage player
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(10);
            }
        }
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);

        if (!hasImpacted)
        {
            TriggerImpact();
        }
    }

    private void TriggerImpact()
    {
        if (hasImpacted) return;
        hasImpacted = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        col.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Impact");
        }
        else
        {
            Destroy(gameObject); // Fallback if no animation
        }
    }

    // Call this from an Animation Event at the end of the impact animation
    public void DestroyAfterImpact()
    {
        Destroy(gameObject);
    }
}

public enum ProjectileType
{
    Player,
    Enemy
}