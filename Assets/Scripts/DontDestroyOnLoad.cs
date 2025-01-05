using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    DDOLManager Manager;
    GameObject[] objects;
    void Awake()
    {
        Manager = GameObject.Find("DontDestroyOnLoadObjectManager").GetComponent<DDOLManager>();
        objects = Manager.objects;
        for (int i = 0; i < objects.Length; i++)
        {
            DontDestroyOnLoad(objects[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
