using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kampaGen : MonoBehaviour
{
    public GameObject kampa;
    //private raycastspawn rcs;
    //private int spawned;
    public int numSpawned = 0;

    public float kampaNum;
    public float howManyKampa;

    public float xrange1;
    public float xrange2;
    public float zrange1;
    public float zrange2;
    public float yrange1;
    public float yrange2;

    public float xpos;
    public float zpos;
    public float ypos;

    //public GameObject load;

    void Start()
    {
        //rcs = thetrees.GetComponent<raycastspawn>();
        //spawned = rcs.numSpawned;

        StartCoroutine(kampaGenerator());
        //Invoke("wait", 3f);
    }

    IEnumerator kampaGenerator()
    {
        while (numSpawned < howManyKampa)
        {
            xpos = Random.Range(xrange1, xrange2);
            zpos = Random.Range(zrange1, zrange2);
            ypos = Random.Range(yrange1, yrange2);

            GameObject Clone = Instantiate(kampa, new Vector3(xpos, ypos, zpos), Quaternion.identity);
            yield return new WaitForSeconds(0.00001f);//waits for one second to spawn
            numSpawned += 1;
        }
    }

    //void wait()
    //{
       // load.active = false;
    //}
}