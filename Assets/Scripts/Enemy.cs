using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        Attack,
        Hit,
        Dead
    }

    [Header("Stats")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private State currentState = State.Patrol;

    private NavMeshAgent agent;
    private Animator animator;

    private Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;

        if (patrolPoints.Length > 0)
            MoveToNextPatrolPoint();
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                PatrolBehavior(distanceToPlayer);
                break;

            case State.Chase:
                ChaseBehavior(distanceToPlayer);
                break;

            case State.Attack:
                AttackBehavior(distanceToPlayer);
                break;
        }
    }

    private void PatrolBehavior(float distanceToPlayer)
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            MoveToNextPatrolPoint();

        if (distanceToPlayer <= detectionRange)
            SwitchState(State.Chase);
    }

    private void ChaseBehavior(float distanceToPlayer)
    {
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (distanceToPlayer <= attackRange)
            SwitchState(State.Attack);

        if (distanceToPlayer > detectionRange + 3f)
            SwitchState(State.Patrol);
    }

    private void AttackBehavior(float distanceToPlayer)
    {
        agent.ResetPath();
        animator.SetFloat("Speed", 0);
        transform.LookAt(player);

        if (distanceToPlayer > attackRange)
        {
            SwitchState(State.Chase);
            return;
        }

        if (Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            // If player has health script:
            // player.GetComponent<PlayerHealth>().TakeDamage(5);
        }
    }

    private void SwitchState(State newState)
    {
        currentState = newState;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // ------------------- DAMAGE & DEATH -------------------

    public void TakeDamage(int dmg)
    {
        if (currentState == State.Dead) return;

        currentHealth -= dmg;

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        currentState = State.Dead;

        if (animator != null)
            animator.SetTrigger("Die");

        agent.enabled = false;
        Destroy(gameObject, 3f); // Wait for death animation
    }
}
