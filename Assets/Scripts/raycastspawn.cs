using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastspawn : MonoBehaviour
{

    public GameObject thetrees;
    private kampaGen tg;
    //private int spawned;

    public float raycastDistance = 50f;
    public GameObject[] list;
    public float overlapTestBoxSize = 5f;
    public LayerMask spawnedObjectLayer;
    

    void Start()
    {
        tg = thetrees.GetComponent<kampaGen>();
        //spawned = tg.numSpawned;

        PositionRaycast();
    }

    void PositionRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {

            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 overlapTestBoxScale = new Vector3(overlapTestBoxSize, overlapTestBoxSize, overlapTestBoxSize);
            Collider[] collidersInsideOverlapBox = new Collider[1];
            int numberOfCollidersFound = Physics.OverlapBoxNonAlloc(hit.point, overlapTestBoxScale, collidersInsideOverlapBox, spawnRotation, spawnedObjectLayer);

            if (numberOfCollidersFound == 0)
            {
                Debug.Log("spawned tree");
                pick(hit.point, spawnRotation);
            }
            else
            {
                Debug.Log("name of collider 0 found " + collidersInsideOverlapBox[0].name);
            }
        }
    }

    void pick(Vector3 positionToSpawn, Quaternion rotationToSpawn)
    {
        int randomIndex = Random.Range(0, list.Length);
        GameObject clone = Instantiate(list[randomIndex], positionToSpawn, rotationToSpawn);
        tg.numSpawned = tg.numSpawned + 1;
    }
}
