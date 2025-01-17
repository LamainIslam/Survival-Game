using UnityEngine;
using UnityEngine.AI;

public class AIScriptHostile : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;
    public float maxHealth;
    public Vector3 spawnLocation;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    GameObject damagePrefab;

    private void Awake()
    {

        agent = GetComponent<NavMeshAgent>();     //asigns a nav mesh agent for ground interaction
        spawnLocation = this.gameObject.transform.position;
        player = GameObject.Find("Player").transform;
        this.gameObject.name = "Hostile Enemy"; //asigns enemy name

        //finding damage prefab using Resources.Load
        damagePrefab = (GameObject)Resources.Load("DamageTaken", typeof(GameObject)) as GameObject;
        if (damagePrefab != null) { Debug.Log("found damagePrefab"); }
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange) Patroling();    
        if (playerInSightRange && !playerInAttackRange) ChasePlayer(); 
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(spawnLocation[0] + randomX, spawnLocation[1], spawnLocation[2] + randomZ);  //sets the movement of the enemy to random directions w/ random x,y,z values

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
            //Attack code
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        //display damage taken
        GameObject damageTaken = Instantiate(damagePrefab, transform.position + Vector3.up * 0, Quaternion.identity);
        damageTaken.GetComponent<FloatingDamageText>().Intialize(damage, Color.red);
    }

    private void OnDrawGizmosSelected()    
    {
        Gizmos.color = Color.red;  //changes the gizmos.color to red and draws a red/yellow wireframe sphere at the enemy's position with radius equal to attackRange.
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
