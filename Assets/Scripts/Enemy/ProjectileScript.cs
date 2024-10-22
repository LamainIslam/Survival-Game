using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private int damage; // No need to set here, it will be set from the AI script

    // Method to set the damage value from the AI script
    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hit is the player
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // Deal damage to the player
                player.TakeDamage(damage);
            }

            // Destroy the projectile after dealing damage
            Destroy(gameObject);
        }

        // Optionally, destroy if it hits anything else
        Destroy(gameObject, 5f); // Destroy after 5 seconds if it doesn't hit the player
    }
}



