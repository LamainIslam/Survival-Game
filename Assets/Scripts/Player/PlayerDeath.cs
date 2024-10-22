using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    //can be accesed from other scripts to kill thge player
    static public bool shouldDie;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //change should die to true to kill the player
        shouldDie = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldDie || rb.position.y < -100) {
            Debug.Log("Died!");
            KillPlayer();
        }
    }

    private void KillPlayer(){
        //currently just retarts the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
