using UnityEngine;
using UnityEngine.AI;

public class AIScriptNeutral : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround;
    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //States
    public float sightRange;
    public bool playerInSightRange;

    private void Awake()
    {
      
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, LayerMask.GetMask("Player"));

        if (!playerInSightRange) Patroling();
        else Flee();
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

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void Flee()
    {
        Vector3 directionToPlayer = transform.position - player.position;
        Vector3 newPos = transform.position + directionToPlayer;
        agent.SetDestination(newPos);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Destroy(gameObject);
    }
}

