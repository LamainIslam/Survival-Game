using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    // Public variable to allow other scripts to kill player
    static public bool shouldDie;
    Rigidbody rb;
    public DDOLManager Whatever;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shouldDie = false;
        Whatever = GameObject.Find("DontDestroyOnLoadObjectManager").GetComponent<DDOLManager>();
    }

    void Update()
    {
        // Kills player if they should die or if they have fallen off map
        if (shouldDie || rb.position.y < -100) {
            Debug.Log("Died!");
            Debug.Log(shouldDie);
            Debug.Log(rb.position.y);
            KillPlayer();
        }
    }

    // Code for killing player (for prototype it just resets scene)
    private void KillPlayer(){
        //resets anything thats need to reset
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //loads into where needed
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Whatever.disable();
        SceneManager.LoadScene("EndMenu");
        shouldDie = false;

    }
}
