using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YetiScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform playerTarget; // Reference to the empty GameObject attached to the player
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public float maxHealth;
    public Vector3 spawnLocation;

    // Patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Jump Attack
    public float timeBetweenAttacks; // Delay between jumps
    private bool alreadyAttacked;
    public float damage = 10f; // Damage dealt to the player on collision

    // States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    private Animator animator; // Reference to the Animator for jumping animation

    private void Awake()
    {
        // Assign components and initialise variables
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spawnLocation = transform.position;

        // Find player and target references
        GameObject playerObject = GameObject.Find("Player");
        GameObject playerTargetObject = GameObject.Find("PlayerTarget");

        if (playerObject != null) player = playerObject.transform;
        if (playerTargetObject != null) playerTarget = playerTargetObject.transform;

        gameObject.name = "Hostile Jumping Enemy";

        if (player == null || playerTarget == null)
        {
            Debug.LogError("Player or PlayerTarget not found! Ensure both exist in the scene.");
        }
    }

    private void Update()
    {
        if (player == null) return; // Ensure player exists

        // Update state checks
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false; // Walk point reached
        }
    }

    private void SearchWalkPoint()
    {
        // Generate random walk point within range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(spawnLocation.x + randomX, spawnLocation.y, spawnLocation.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        // Stop moving
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            JumpTowardPlayerTarget();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void JumpTowardPlayerTarget()
    {
        if (playerTarget == null) return; // Ensure target exists

        // Look at the player target
        transform.LookAt(new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z));

        // Trigger jump animation
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        // Move towards the player's position
        agent.SetDestination(player.position);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject); // Destroy enemy when health is zero
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player playerScript = collision.collider.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage((int)damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}