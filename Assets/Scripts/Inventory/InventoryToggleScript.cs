using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggleScript : MonoBehaviour
{
    public GameObject mainInventoryGroup;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            if(mainInventoryGroup.activeInHierarchy == false){
                mainInventoryGroup.SetActive(true);
                GameObject.Find("PlayerCameraHolder").transform.GetChild(0).GetComponent<PlayerCamera>().lockCursor = false;
            }else {
                mainInventoryGroup.SetActive(false);
                GameObject.Find("PlayerCameraHolder").transform.GetChild(0).GetComponent<PlayerCamera>().lockCursor = true;
            }
        }
    }
}
