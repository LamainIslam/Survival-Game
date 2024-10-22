using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    // Public variable to allow other scripts to kill player
    static public bool shouldDie;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shouldDie = false;
    }

    void Update()
    {
        // Kills player if they should die or if they have fallen off map
        if (shouldDie || rb.position.y < -100) {
            Debug.Log("Died!");
            KillPlayer();
        }
    }

    // Code for killing player (for prototype it just resets scene)
    private void KillPlayer(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
