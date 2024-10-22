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

    // Changes selected slot to the slot clicked if in toolbar
    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent.name == "Toolbar")
        {
            inventoryManager.ChangeSelectedSlot(transform.GetSiblingIndex());
        }
    }

    // Changes slot to seleted colour
    public void Select()
    {
        image.color = selectedColor;
    }

    // Changes slot to non selected colour
    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    // Lets you drop item in correct slot
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

    // Handles generic item slots
    private void HandleGenericSlot(InventoryItem heldItem)
    {
        InventoryItem hoveredItem = GetItemInSlot();

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

    // Handles armour item slots
    private void HandleArmorSlot(InventoryItem heldItem, ItemType expectedType)
    {
        if (heldItem.item.type == expectedType)
        {
            InventoryItem hoveredItem = GetItemInSlot();

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

    // Handles offhand item slot
    private void HandleOffHandSlot(InventoryItem heldItem)
    {
        if (heldItem.item.type == ItemType.Torch || 
            heldItem.item.type == ItemType.Shield || 
            heldItem.item.type == ItemType.Food)
        {
            InventoryItem hoveredItem = GetItemInSlot(); 

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

    // Swaps any 2 items
    private void SwapItems(InventoryItem heldItem, InventoryItem hoveredItem)
    {
        Transform originalParent = heldItem.parentAfterDrag;
        heldItem.parentAfterDrag = hoveredItem.transform.parent;
        hoveredItem.transform.SetParent(originalParent);
    }

    // Merges the stakcs of items
    private void MergeStacks(InventoryItem heldItem, InventoryItem hoveredItem)
    {
        if (heldItem.item.stackable && hoveredItem.count < inventoryManager.stackSize)
        {
            int remainingSpace = inventoryManager.stackSize - hoveredItem.count;
            if (remainingSpace >= heldItem.count)
            {
                hoveredItem.count += heldItem.count;
                hoveredItem.RefreshCount();
                Destroy(heldItem.gameObject); 
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
    public InventoryItem GetItemInSlot()

    {
        foreach (Transform child in transform)
        {
            InventoryItem item = child.GetComponent<InventoryItem>();
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }

}
