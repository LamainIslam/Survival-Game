using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Added for IEnumerator


public class PlainsBoss : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    // Boss Attributes
    public float maxHealth = 300f;
    public float currentHealth;

    // Charge Mechanic
    public float chargeSpeed = 10f;
    public float chargeCooldown = 5f;
    private bool isCharging;
    private bool canCharge = true;

    // Blocking Mechanic
    public float blockDuration = 3f;
    private bool isBlocking;

    // Detection
    public float sightRange = 20f;
    private bool playerInSightRange;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        // Check if the player is within range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!isBlocking && playerInSightRange && !isCharging && canCharge)
        {
            StartCoroutine(ChargeAtPlayer());
        }
    }

    private IEnumerator ChargeAtPlayer()
    {
        // Start the charge
        isCharging = true;
        canCharge = false;

        agent.speed = chargeSpeed;
        agent.SetDestination(player.position);

        yield return new WaitForSeconds(2f); // Duration of the charge

        // Reset agent speed and state
        agent.speed = 3.5f; // Normal movement speed
        isCharging = false;

        // Enter blocking mode for a duration
        StartCoroutine(BlockAttacks());

        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    private IEnumerator BlockAttacks()
    {
        isBlocking = true;
        Debug.Log("Boss is blocking!");

        // Block animation can be triggered here
        yield return new WaitForSeconds(blockDuration);

        isBlocking = false;
        Debug.Log("Boss stopped blocking.");
    }

    public void TakeDamage(float damage)
    {
        if (isBlocking)
        {
            Debug.Log("Attack blocked!");
            return; // Ignore damage while blocking
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

