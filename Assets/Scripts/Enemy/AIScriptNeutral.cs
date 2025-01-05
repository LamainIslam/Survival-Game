using UnityEngine;
using UnityEngine.AI;

public class AIScriptNeutral : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public float maxHealth;
    public Vector3 spawnLocation;
    public bool aggressive;  // Determines if the neutral enemy has become aggressive
    public Material aggressiveMaterial;  // Material to visually indicate the aggressive state

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    GameObject damagePrefab;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();  // Assign NavMeshAgent to control movement
        spawnLocation = this.gameObject.transform.position;  // Set the spawn point for patrols
        aggressive = false;  // Neutral state by default
        player = GameObject.Find("Player").transform;  // Find and reference the player object
        this.gameObject.name = "Neutral Enemy";  // Name the game object for identification

        //finding damage prefab using Resources.Load
        damagePrefab = (GameObject)Resources.Load("DamageTaken", typeof(GameObject)) as GameObject;
        if (damagePrefab != null) { Debug.Log("found damagePrefab"); }
    }

    private void Update()
    {
        // Check for player within sight and attack range using spheres
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange)
            Patroling();  // Patrol if the player is not in sight

        // If aggressive, start attacking behaviour when player is in sight
        if (aggressive)
        {
            if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();  // Chase if in sight but not within attack range
            if (playerInAttackRange && playerInSightRange)
                AttackPlayer();  // Attack when within attack range
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();  // Find a new patrol point if none is set

        if (walkPointSet)
            agent.SetDestination(walkPoint);  // Move towards the patrol point

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Reset walk point when the enemy reaches its destination
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Generate random patrol points within the set range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(spawnLocation[0] + randomX, spawnLocation[1], spawnLocation[2] + randomZ);

        // Ensure the walk point is on the ground before setting it
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        // Move towards the player if in sight
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);  // Stop moving while attacking
        transform.LookAt(player);  // Face the player before attacking

        if (!alreadyAttacked)
        {
            // Instantiate projectile and add force for attack
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);  // Forward force
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);  // Upward force for trajectory

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);  // Set a cooldown between attacks
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;  // Reset the attack state to allow for future attacks
    }

    public void TakeDamage(float damage)
    {
        health -= damage;  // Reduce health when damage is taken

        //display damage taken
        GameObject damageTaken = Instantiate(damagePrefab, transform.position + Vector3.up * 0, Quaternion.identity);
        damageTaken.GetComponent<FloatingDamageText>().Intialize(damage, Color.red);

        // If the enemy was neutral, it becomes aggressive when damaged
        if (aggressive == false)
        {
            aggressive = true;  // Set to aggressive
            GetComponent<Renderer>().material = aggressiveMaterial;  // Change material to indicate aggression
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red wire sphere for the attack range and yellow for sight range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
