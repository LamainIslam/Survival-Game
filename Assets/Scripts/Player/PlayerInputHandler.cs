using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private InteractWithItem interactWithItem;
    private UseItem useItem;
    private InventoryManager inventoryManager;

    private bool isPrimaryHeld = false;
    public float primaryHoldTime = 0f;
    public float maxMultiplier = 50f; // Cap for damage multiplier

    void Start()
    {
        // Assign variables
        interactWithItem = GetComponent<InteractWithItem>();
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
        if (Input.GetKeyDown(KeyCode.F)) {
            if (interactWithItem != null) {
                interactWithItem.TryInteractWithItem();
            }
        }

        // Toggle inventory
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryManager.ToggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventoryManager.DropItem();
        }

        // Handle primary attack
        if (Input.GetMouseButtonDown(0) && !inventoryManager.mainInventoryGroup.activeInHierarchy)
        {
            isPrimaryHeld = true;
            primaryHoldTime = 0f; // Reset hold time
        }

        if (Input.GetMouseButton(0) && isPrimaryHeld)
        {
            primaryHoldTime += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && isPrimaryHeld)
        {
            float damageMultiplier = Mathf.Min(1f + primaryHoldTime*4, maxMultiplier);
            useItem.TryUsePrimaryWithMultiplier(damageMultiplier);
            isPrimaryHeld = false; // Reset state
        }

        // Handle offhand item
        if (Input.GetMouseButtonDown(1) && !inventoryManager.mainInventoryGroup.activeInHierarchy)
        {
            useItem.TryUseOffHandItem();
        }
    }
}
