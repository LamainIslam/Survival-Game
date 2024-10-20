using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PickupItem pickupItem;
    private UseItem useItem;
    private InventoryManager inventoryManager;

    void Start()
    {
        // Assign variables
        pickupItem = GetComponent<PickupItem>();
        useItem = GetComponent<UseItem>();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Check for the F key to pick up items
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (pickupItem != null)
            {
                pickupItem.TryPickupItem();
            }
        }

        // Check for the E key to toggle the inventory
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryManager.ToggleInventory();
        }

        // Check for the E key to toggle the inventory
        if (Input.GetMouseButtonDown(0))
        {
            useItem.TryUseItem();
        }
    }
}