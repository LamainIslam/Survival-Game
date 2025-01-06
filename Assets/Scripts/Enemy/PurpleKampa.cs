using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PurpleKampa : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    public float maxHealth;
    public Vector3 spawnLocation;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private GameObject damagePrefab;

    // Animator and states
    public Animator animator;
    private enum EnemyState { Base, Walk, Run, Attack }
    private EnemyState currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnLocation = transform.position;
        player = GameObject.Find("Player").transform;
        this.gameObject.name = "Aggressive Enemy";

        // Load damage prefab
        damagePrefab = (GameObject)Resources.Load("DamageTaken", typeof(GameObject)) as GameObject;
        if (damagePrefab != null) Debug.Log("Found damagePrefab");

        animator = GetComponent<Animator>(); // Assign the Animator component
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
            ChangeAnimationState(EnemyState.Walk);
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            ChangeAnimationState(EnemyState.Run);
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
            ChangeAnimationState(EnemyState.Attack);
        }
    }

    private void ChangeAnimationState(EnemyState newState)
    {
        if (currentState == newState) return; // Avoid redundant state changes
        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Base:
                animator.SetInteger("aniType", 0);
                break;
            case EnemyState.Walk:
                animator.SetInteger("aniType", 1);
                break;
            case EnemyState.Run:
                animator.SetInteger("aniType", 2);
                break;
            case EnemyState.Attack:
                animator.SetInteger("aniType", 3);
                break;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(spawnLocation.x + randomX, spawnLocation.y, spawnLocation.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position); // Stop moving
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            DealMeleeDamage();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void DealMeleeDamage()
    {
        if (playerInAttackRange)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(10); // Deal 10 damage to the player
            }

            Debug.Log("Melee attack dealt to the player!");
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Display damage taken
        GameObject damageTaken = Instantiate(damagePrefab, transform.position + Vector3.up, Quaternion.identity);
        damageTaken.GetComponent<FloatingDamageText>().Intialize(damage, Color.red);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
