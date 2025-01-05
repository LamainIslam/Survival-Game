using UnityEngine;
using UnityEngine.AI;

public class AIScriptPassive : MonoBehaviour
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

    // States
    public float sightRange;
    public bool playerInSightRange;

    GameObject damagePrefab;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();  // Reference to NavMeshAgent for movement
        spawnLocation = this.gameObject.transform.position; // Store the spawn position for patrol reference
        player = GameObject.Find("Player").transform; // Locate and assign the player object
        this.gameObject.name = "Passive Enemy"; // Name for easier identification in the editor

        //finding damage prefab using Resources.Load
        damagePrefab = (GameObject)Resources.Load("DamageTaken", typeof(GameObject)) as GameObject;
        if (damagePrefab != null) { Debug.Log("found damagePrefab"); }
    }

    private void Update()
    {
        // Check if the player is within sight range (using an invisible detection sphere)
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
        {
            Patroling(); // Wander around if the player is not detected
        }
        else
        {
            FleePlayer(); // Flee when the player is in sight
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();  // Find a new point to move to if none is set

        if (walkPointSet)
            agent.SetDestination(walkPoint); // Move towards the set walk point

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Reset walk point if close enough, so a new one can be set
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Randomly generate a new point in a square range around the spawn location
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(spawnLocation[0] + randomX, spawnLocation[1], spawnLocation[2] + randomZ);

        // Check if the new walk point is on the ground using a downward raycast
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true; // Set the walk point only if it is valid
    }

    private void FleePlayer()
    {
        // Calculate a position directly opposite the player's current position
        Vector3 newPosition = new Vector3(2 * transform.position.x - player.position.x, transform.position.y, 2 * transform.position.z - player.position.z);

        // Move the enemy to this new position away from the player
        agent.SetDestination(newPosition);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;  // Reduce health by the amount of damage taken

        //display damage taken
        GameObject damageTaken = Instantiate(damagePrefab, transform.position + Vector3.up * 0, Quaternion.identity);
        damageTaken.GetComponent<FloatingDamageText>().Intialize(damage, Color.red);
    }
}
