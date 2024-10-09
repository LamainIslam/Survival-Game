using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggleScript : MonoBehaviour
{
    public GameObject mainInventoryGroup;

    void Update()
    {
        // Toggle inventory when clicking 'e'
        if (Input.GetKeyDown("e"))
        {
            if(mainInventoryGroup.activeInHierarchy == false){
                mainInventoryGroup.SetActive(true);
            }else {
                mainInventoryGroup.SetActive(false);
            }
        }
    }
}
