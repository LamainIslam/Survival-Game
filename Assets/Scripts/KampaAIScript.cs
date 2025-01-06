using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KampaAIScript : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    public float maxHealth;
    public Vector3 spawnLocation;
    public bool aggressive;
    public Material aggressiveMaterial;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float attackRange;
    public float sightRange;

    public bool playerInSightRange, playerInAttackRange;

    private GameObject damagePrefab;

    public Animator animator; // Reference to the Animator component

    // Enemy States
    private enum EnemyState { Base, Walk, Run, Attack }
    private EnemyState currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnLocation = transform.position;
        aggressive = false;
        player = GameObject.Find("Player").transform;
        this.gameObject.name = "Kampa Enemy";

        // Load damage prefab
        damagePrefab = (GameObject)Resources.Load("DamageTaken", typeof(GameObject)) as GameObject;
        if (damagePrefab != null) Debug.Log("Found damagePrefab");

        animator = GetComponent<Animator>(); // Assign the Animator component
    }

    private void Start()
    {
        // Ensure EnemyManager knows this enemy
        KampaManagerScript.Instance.RegisterEnemy(this);
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange) AttackPlayer();

        // Example: Change animation state based on conditions
        if (playerInAttackRange)
        {
            ChangeAnimationState(EnemyState.Attack);
        }
        else if (playerInSightRange)
        {
            ChangeAnimationState(EnemyState.Run);
        }
        else
        {
            ChangeAnimationState(EnemyState.Walk);
        }
    }

    private void ChangeAnimationState(EnemyState newState)
    {
        if (currentState == newState) return; // Avoid redundant changes

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
        agent.SetDestination(transform.position);
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

    public void BecomeAggressive()
    {
        if (!aggressive)
        {
            aggressive = true;
            GetComponent<Renderer>().material = aggressiveMaterial; // Change material to indicate aggression

            Debug.Log($"{gameObject.name} has become aggressive!");

        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        GameObject damageTaken = Instantiate(damagePrefab, transform.position + Vector3.up * 1, Quaternion.identity);
        damageTaken.GetComponent<FloatingDamageText>().Intialize(damage, Color.red);

        if (!aggressive)
        {
            aggressive = true;
            GetComponent<Renderer>().material = aggressiveMaterial;

            // Notify other enemies
            KampaManagerScript.Instance.AlertAllEnemies();
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
