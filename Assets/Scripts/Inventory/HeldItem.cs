using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public Item heldItem;
    private GameObject heldItemInstance;

    // Hold selected item
    public void HoldItem(Item newItem)
    {
        if (heldItemInstance != null) {
            Destroy(heldItemInstance);
        }
        heldItem = newItem;
        if (heldItem != null) {
            heldItemInstance = Instantiate(heldItem.prefab, transform); 
            heldItemInstance.transform.SetParent(gameObject.transform);
        }
    }
}
