using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public GameObject[] objects;
    void Awake()
    {
        for(int i = 0; i < objects.Length; i++)
        {
            DontDestroyOnLoad(objects[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
