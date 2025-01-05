using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class SnowmanEnemy : MonoBehaviour
{
    public Animator animator; // Assign in the Inspector
    public float damageAmount = 10f;
    public float damageInterval = 1f; // Damage every second
    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("Attack"); // Start attack animation
            isPlayerInRange = true;
            StartCoroutine(DamagePlayerOverTime(other.GetComponent<PlayerHealth>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            StopAllCoroutines(); // Stop damaging the player
        }
    }

    private IEnumerator DamagePlayerOverTime(PlayerHealth playerHealth)
    {
        while (isPlayerInRange)
        {
            playerHealth.TakeDamage(damageAmount); // Reduce player's health
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
