using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class YetiScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

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

    // Player Interaction
    private Vector3 lastKnownPlayerPosition;
    private bool isChasingPlayer;

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
        // Yeti patrol behaviour can be implemented here if needed (e.g., wandering in a specific area)
        agent.SetDestination(agent.transform.position); // Yeti stays idle when not chasing or attacking
    }

    private void ChasePlayer()
    {
        if (!isChasingPlayer)
        {
            lastKnownPlayerPosition = player.position;
            isChasingPlayer = true;
        }

        agent.SetDestination(lastKnownPlayerPosition);

        // Stop chasing if reached last known player position
        if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1f)
        {
            isChasingPlayer = false;
        }
    }

    private void AttackPlayer()
    {
        // Stop moving when attacking
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
