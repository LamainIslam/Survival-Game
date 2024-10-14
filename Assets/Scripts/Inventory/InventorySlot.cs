using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;
    public SlotType slotType;

    public enum SlotType
    {
        None,
        Helmet,
        Chestplate,
        Leggings,
        Boots,
        OffHand
    }

    [HideInInspector] public InventoryManager inventoryManager;

    private void Awake()
    {
        Deselect();
    }

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent.name == "Toolbar")
        {
            inventoryManager.ChangeSelectedSlot(transform.GetSiblingIndex());
        }
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (heldItem == null) return;

        switch (slotType)
        {
            case SlotType.None:
                HandleGenericSlot(heldItem);
                break;
            case SlotType.Helmet:
                HandleArmorSlot(heldItem, ItemType.Helmet);
                break;
            case SlotType.Chestplate:
                HandleArmorSlot(heldItem, ItemType.Chestplate);
                break;
            case SlotType.Leggings:
                HandleArmorSlot(heldItem, ItemType.Leggings);
                break;
            case SlotType.Boots:
                HandleArmorSlot(heldItem, ItemType.Boots);
                break;
            case SlotType.OffHand:
                HandleOffHandSlot(heldItem);
                break;
        }
    }

    private void HandleGenericSlot(InventoryItem heldItem)
    {
        InventoryItem hoveredItem = GetItemInSlot();  // Get the actual InventoryItem in the slot

        if (hoveredItem == null)
        {
            heldItem.parentAfterDrag = transform;
        }
        else
        {
            if (heldItem.item != hoveredItem.item)
            {
                SwapItems(heldItem, hoveredItem);
            }
            else
            {
                MergeStacks(heldItem, hoveredItem);
            }
        }
    }

    private void HandleArmorSlot(InventoryItem heldItem, ItemType expectedType)
    {
        if (heldItem.item.type == expectedType)
        {
            InventoryItem hoveredItem = GetItemInSlot();  // Get the actual InventoryItem in the slot

            if (hoveredItem == null)
            {
                heldItem.parentAfterDrag = transform;
            }
            else
            {
                SwapItems(heldItem, hoveredItem);
            }
        }
    }

    private void HandleOffHandSlot(InventoryItem heldItem)
    {
        // Check if the item is one of the allowed types for the OffHand slot
        if (heldItem.item.type == ItemType.Torch || 
            heldItem.item.type == ItemType.Shield || 
            heldItem.item.type == ItemType.Food)
        {
            InventoryItem hoveredItem = GetItemInSlot();  // Get the actual InventoryItem in the slot

            if (hoveredItem == null)
            {
                heldItem.parentAfterDrag = transform;  // Equip off-hand item in empty slot
            }
            else
            {
                SwapItems(heldItem, hoveredItem);  // Swap off-hand item
            }
        }
    }

    private void SwapItems(InventoryItem heldItem, InventoryItem hoveredItem)
    {
        Transform originalParent = heldItem.parentAfterDrag;
        heldItem.parentAfterDrag = hoveredItem.transform.parent;
        hoveredItem.transform.SetParent(originalParent);
    }

    private void MergeStacks(InventoryItem heldItem, InventoryItem hoveredItem)
    {
        if (heldItem.item.stackable && hoveredItem.count < inventoryManager.stackSize)
        {
            int remainingSpace = inventoryManager.stackSize - hoveredItem.count;
            if (remainingSpace >= heldItem.count)
            {
                hoveredItem.count += heldItem.count;
                hoveredItem.RefreshCount();
                Destroy(heldItem.gameObject);  // Destroy held item after merging
            }
            else
            {
                heldItem.count -= remainingSpace;
                hoveredItem.count = inventoryManager.stackSize;
                hoveredItem.RefreshCount();
                heldItem.RefreshCount();
            }
        }
    }

    private InventoryItem GetItemInSlot()
    {
        // Loop through children to find the InventoryItem component
        foreach (Transform child in transform)
        {
            InventoryItem item = child.GetComponent<InventoryItem>();
            if (item != null)
            {
                return item;  // Return the first found InventoryItem
            }
        }
        return null;  // No InventoryItem found
    }
}
