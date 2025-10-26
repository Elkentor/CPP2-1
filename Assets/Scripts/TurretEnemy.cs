using UnityEngine;

public class TurretEnemy : Enemy
{
    [SerializeField] private float fireRate = 3.0f;
    [SerializeField] private float detectionRadius = 5.0f;
    private float timeSinceLastShot = 0.0f;

    private Transform player;
    private Shoot shoot;

    protected override void Start()
    {
        base.Start();

        if (fireRate <= 0)
        {
            Debug.LogError("Fire rate must be greater than 0. Setting to default value of 2.0f.");
            fireRate = 2.0f;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        shoot = GetComponent<Shoot>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Idle"))
            {
                if (Time.time >= timeSinceLastShot + fireRate)
                {
                    // Flip turret based on player position
                    SpriteRenderer sr = shoot.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.flipX = player.position.x < transform.position.x;
                    }

                    anim.SetTrigger("Fire");
                    timeSinceLastShot = Time.time;

                    shoot?.Fire();

                    Debug.Log($"{name} fired at {Time.time} (Player distance: {distance})");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
