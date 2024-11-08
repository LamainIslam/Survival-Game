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

            Rigidbody rb = heldItemInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }
            BoxCollider collider = heldItemInstance.GetComponent<BoxCollider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            heldItemInstance.transform.SetParent(gameObject.transform);
        }
    }
}
