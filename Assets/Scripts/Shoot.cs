using UnityEngine;

public class Shoot : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private Vector2 initShotVelocity = Vector2.zero;
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;
    [SerializeField] private Projectile projectilePrefab = null;

    private Vector2 leftShotVel = Vector2.zero;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (initShotVelocity == Vector2.zero)
        {
            initShotVelocity = new Vector2(10f, 0f); // Default shot velocity if not set
            Debug.LogWarning("Initial shot velocity not set. Using default value: " + initShotVelocity);
        }

        if (leftSpawn == null || rightSpawn == null || projectilePrefab == null)
        {
            Debug.LogError("Spawn points or projectile prefab not set. Please assign in the inspector.");
        }

        // Set the left and right shot velocities based on the initial shot velocity
        leftShotVel = new Vector2(-initShotVelocity.x, initShotVelocity.y);
    }

    public void Fire()
    {
        Projectile curProjectile;

        if (!sr.flipX)
        {
            curProjectile = Instantiate(projectilePrefab, rightSpawn.position, Quaternion.identity);
            curProjectile.SetVelocity(initShotVelocity);
        }
        else
        {
            curProjectile = Instantiate(projectilePrefab, leftSpawn.position, Quaternion.identity);
            curProjectile.SetVelocity(leftShotVel);

            // Flip the projectile sprite when shooting left
            SpriteRenderer projSR = curProjectile.GetComponent<SpriteRenderer>();
            if (projSR != null)
            {
                projSR.flipX = true;
            }
        }
    }
}
