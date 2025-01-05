using System.Collections;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public float maxDistance = 2.5f;
    private Camera mainCamera;
    private InventoryManager inventoryManager;
    private HeldItem heldItemComponent;
    private RaycastHit hit;

    void Start()
    {
        // Assign variables
        mainCamera = Camera.main;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        heldItemComponent = GameObject.Find("HeldItem").GetComponent<HeldItem>();
    }

    // Handle item pickup logic (called externally by PlayerInputHandler)
    public void TryPickupItem()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.CompareTag("Item"))
        {
            // Add item to inventory and update held item
            var itemHolder = hit.collider.gameObject.GetComponent<ItemHolder>();
            bool canAddItem = inventoryManager.AddItem(itemHolder.item);
            if(canAddItem == true) {
                heldItemComponent.heldItem = inventoryManager.GetSelectedItem();
                heldItemComponent.HoldItem(heldItemComponent.heldItem);

                // Destroy item
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
