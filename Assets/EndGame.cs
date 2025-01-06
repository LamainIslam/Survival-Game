using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemies; // List of enemies to check

    [Header("Portal Settings")]
    public string sceneToLoad; // Name of the scene to load
    public bool allEnemiesDefeated = false; // Tracks if all enemies are dead

    private void Update()
    {
        CheckEnemies();
    }

    void CheckEnemies()
    {
        allEnemiesDefeated = true; // Assume all enemies are dead initially

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null) // If any enemy is still alive
            {
                allEnemiesDefeated = false;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        if (other.CompareTag("Player")) // Ensure only the player can trigger the portal
        {
            if (allEnemiesDefeated)
            {
                LoadNextScene();
            }
            else
            {
                Debug.Log("Defeat all enemies before using the portal!");
            }
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("No scene specified to load!");
        }
    }
}