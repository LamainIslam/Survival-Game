using UnityEngine;
using UnityEngine.AI;

public class YetiScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    // Patrol
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange = 10f;

    // Health
    public float maxHealth = 150f;
    public float currentHealth;

    // Detection and Attack
    public float sightRange = 15f;
    public float attackRange = 3f;
    public float attackDamage = 45f;
    public float attackCooldown = 2f;
    private bool playerInSightRange, playerInAttackRange;
    private bool alreadyAttacked;

    // Movement
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        // Check for player in sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange) AttackPlayer();
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        // Check if the walk point is reached
        if (Vector3.Distance(transform.position, walkPoint) < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Generate a random point within the walk range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Check if the point is valid
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Stop moving to attack
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Melee attack logic
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage((int)attackDamage);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }

        // After attacking, chase the player again if they are still in sight range
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            alreadyAttacked = false;
            ChasePlayer();
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Logic for enemy death
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
