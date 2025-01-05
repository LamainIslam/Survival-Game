using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLManager : MonoBehaviour
{
    public GameObject[] objects;
    // Start is called before the first frame update

    
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void disable()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
            objects[i].transform.position = Vector3.zero; // Position at (0, 0, 0)
            objects[i].transform.rotation = Quaternion.identity; // No rotation (identity quaternion)
            objects[i].transform.localScale = Vector3.one; // Scale of (1, 1, 1)
        }
    }

    public void enable()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(true);
            //objects[i].transform.position = Vector3.zero; // Position at (0, 0, 0)
            //objects[i].transform.rotation = Quaternion.identity; // No rotation (identity quaternion)
            //objects[i].transform.localScale = Vector3.one; // Scale of (1, 1, 1)
        }
    }

    /*public void destroyer()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);
        }

    }
    */
   
}
