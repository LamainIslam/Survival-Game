using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    [HideInInspector] public InventoryManager inventoryManager;

    private void Awake() {
        // Starts every slot as deselected colour
        Deselect();
    }

    private void Start() {
        // Initialise inventoryManager
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    // Detects if mouse clicks this slot
    public void OnPointerClick(PointerEventData eventData)
    {
        // Change slot if it's in toolbar
        if (gameObject.transform.parent.name == "Toolbar") {
            inventoryManager.ChangeSelectedSlot(gameObject.transform.GetSiblingIndex());
        }
    }

    // Change to selected colour
    public void Select() {
        image.color = selectedColor;
    }

    // Change to deselected colour
    public void Deselect() {
        image.color = notSelectedColor;
    }

    // Changes parent of item when mouse is released
    public void OnDrop(PointerEventData eventData) {
        if (transform.childCount == 0) {
            // Move item to empty slot
            InventoryItem heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            heldItem.parentAfterDrag = transform;
        }else {
            // Check held item and hovered item
            InventoryItem heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            InventoryItem hoveredItem = transform.GetChild(0).GetComponent<InventoryItem>();

            if (heldItem.item != hoveredItem.item) {
                // Swap items if different items
                Transform heldItemParent = heldItem.parentAfterDrag;
                heldItem.parentAfterDrag = transform;
                heldItem.transform.SetParent(transform);
                hoveredItem.transform.SetParent(heldItemParent);
            }else {
                // Merge stacks if stackable
                if (heldItem.item.stackable == true && hoveredItem.count < inventoryManager.stackSize) {
                    if (inventoryManager.stackSize - hoveredItem.count >= heldItem.count) {
                        hoveredItem.count += heldItem.count;
                        hoveredItem.RefreshCount();
                        Destroy(heldItem.gameObject);
                    }else {
                        heldItem.count += hoveredItem.count - inventoryManager.stackSize;
                        hoveredItem.count = inventoryManager.stackSize;
                        hoveredItem.RefreshCount();
                        heldItem.RefreshCount();
                    }
                }
            }
        }
    }
}
