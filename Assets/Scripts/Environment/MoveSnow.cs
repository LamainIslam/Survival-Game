using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSnow : MonoBehaviour
{
    public Transform playerPosition;

    // Update is called once per frame
    void Update()
    {
       transform.position = playerPosition.position;
    }
}
