using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class STPSceneTeleporter : MonoBehaviour
{
    public GameObject source;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(0);
        Debug.Log("blahblahblah");
    }



}
