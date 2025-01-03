using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PTSSceneTeleporter : MonoBehaviour
{
    public GameObject source;

    private void OnTriggerEnter(Collider other)
    {
        SceneNavigation.lastSelectedScene = 8.ToString();
        SceneManager.LoadScene(8);
    }



}
