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
    public InventoryManager inventoryManager;
    public Player player;

    public enum SlotType
    {
        None,
        Helmet,
        Chestplate,
        Leggings,
        Boots,
        OffHand,
        Fuel,
        Food,
        Empty
    }

    private void Awake()
    {
        // Start all slots as deselected
        Deselect();
    }

    private void Start()
    {
        // Assign inventoryManager
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();

        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Changes selected slot on click or moves item on shift-click
    public void OnPointerClick(PointerEventData eventData)
    {
        bool isShiftClick = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Run either shift-click code or change the selected item
        if (isShiftClick) {
            InventoryItem clickedItem = GetItemInSlot();
            if (clickedItem != null) {
                HandleShiftClick(clickedItem);
            }
        }else {
            if (transform.parent.name == "Toolbar") {
                inventoryManager.ChangeSelectedSlot(transform.GetSiblingIndex());
            }
        }
    }

    // Handles shift-clicking
    private void HandleShiftClick(InventoryItem clickedItem)
    {
        if (slotType == SlotType.OffHand) {
            // If in the offhand slot, move it to the main inventory
            TryMoveItemToMainInventory(clickedItem);
        }else if (clickedItem.item.type == ItemType.Helmet || clickedItem.item.type == ItemType.Chestplate || clickedItem.item.type == ItemType.Leggings || clickedItem.item.type == ItemType.Boots) {
            // If the item is armor, try moving it to the appropriate slot
            if (!TryMoveArmorToSlot(clickedItem)) {
                // If armor slot is full, switch between main and toolbar inventory
                SwitchBetweenMainAndToolbar(clickedItem);
            }
            player.UpdateDefence();
        }else {
            // For all other items, switch between main and toolbar
            SwitchBetweenMainAndToolbar(clickedItem);
        }
        inventoryManager.UpdateHeldItem();
    }

    // Quick equips armour
    private bool TryMoveArmorToSlot(InventoryItem item)
    {
        SlotType armorSlotType = GetArmorSlotType(item.item.type);

        foreach (GameObject slot in inventoryManager.armourSlots)
        {
            if (slot.GetComponent<InventorySlot>().slotType == armorSlotType && slot.GetComponent<InventorySlot>().GetItemInSlot() == null)
            {
                // Move item to the specific empty armour slot
                item.transform.SetParent(slot.transform);
                item.parentAfterDrag = slot.transform;
                return true;
            }
        }
        return false;
    }

    // Helper function for HandleShiftClick() - returns the armour slot type
    private SlotType GetArmorSlotType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Helmet: return SlotType.Helmet;
            case ItemType.Chestplate: return SlotType.Chestplate;
            case ItemType.Leggings: return SlotType.Leggings;
            case ItemType.Boots: return SlotType.Boots;
            default: return SlotType.None;
        }
    }

    // Attempts to swap item to main inventory
    private void TryMoveItemToMainInventory(InventoryItem item)
    {
        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            if (slot.slotType == SlotType.None && slot.GetItemInSlot() == null)
            {
                // Move the item to an empty main inventory slot
                item.transform.SetParent(slot.transform);
                item.parentAfterDrag = slot.transform;
                return;
            }
        }
    }

    // Attempts to switch the item between the main inventory and toolbar
    private void SwitchBetweenMainAndToolbar(InventoryItem item)
    {
        bool isMainInventory = transform.parent.name == "MainInventory";

        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            bool isToolbarSlot = slot.transform.parent.name == "Toolbar";
            bool isEmptySlot = slot.GetItemInSlot() == null;

            // Move item to an empty slot in the other inventory section
            if (isMainInventory && isToolbarSlot && isEmptySlot)
            {
                item.transform.SetParent(slot.transform);
                item.parentAfterDrag = slot.transform;
                return;
            }
            else if (!isMainInventory && !isToolbarSlot && isEmptySlot)
            {
                item.transform.SetParent(slot.transform);
                item.parentAfterDrag = slot.transform;
                return;
            }
        }
    }

    // Changes slot to selected colour
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
        if (heldItem == null) {
            return;
        }

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
            case SlotType.Fuel:
                HandleFuelSlot(heldItem);
                break;
            case SlotType.Food:
                HandleFoodSlot(heldItem);
                break;
            case SlotType.Empty:
                HandleEmptySlot(heldItem);
                break;
        }
    }

    // Handles generic item slots
    private void HandleGenericSlot(InventoryItem heldItem)
    {
        InventoryItem hoveredItem = GetItemInSlot();
        if (hoveredItem == null) {
            heldItem.parentAfterDrag = transform;
        }else {
            if (heldItem.item != hoveredItem.item) {
                SwapItems(heldItem, hoveredItem);
            }else {
                MergeStacks(heldItem, hoveredItem);
            }
        }
    }

    // Handles armour item slots
    private void HandleArmorSlot(InventoryItem heldItem, ItemType expectedType)
    {
        if (heldItem.item.type == expectedType) {
            InventoryItem hoveredItem = GetItemInSlot();
            if (hoveredItem == null) {
                heldItem.parentAfterDrag = transform;
            }else {
                SwapItems(heldItem, hoveredItem);
            }
        }
    }

    // Handles offhand item slot
    private void HandleOffHandSlot(InventoryItem heldItem)
    {
        if (heldItem.item.type == ItemType.Torch || heldItem.item.type == ItemType.Shield || heldItem.item.type == ItemType.Food) {
            InventoryItem hoveredItem = GetItemInSlot(); 
            if (hoveredItem == null) {
                heldItem.parentAfterDrag = transform; 
            }else {
                SwapItems(heldItem, hoveredItem);
            }
        }
    }





    private void HandleFuelSlot(InventoryItem heldItem)
    {
        if (heldItem.item.type == ItemType.Resource)
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

    private void HandleFoodSlot(InventoryItem heldItem)
    {
        if (heldItem.item.type == ItemType.Food)
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

    private void HandleEmptySlot(InventoryItem heldItem)
    {
        // Prevent any items from being dropped into this slot
        Debug.Log("This slot is reserved and cannot hold items.");
    }





    // Swaps any 2 items
    private void SwapItems(InventoryItem heldItem, InventoryItem hoveredItem)
    {
        Transform originalParent = heldItem.parentAfterDrag;
        heldItem.parentAfterDrag = hoveredItem.transform.parent;
        hoveredItem.transform.SetParent(originalParent);
        
    }

    // Merges the stacks of items
    private void MergeStacks(InventoryItem heldItem, InventoryItem hoveredItem)
    {
        if (heldItem.item.stackable && hoveredItem.count < inventoryManager.stackSize) {
            int remainingSpace = inventoryManager.stackSize - hoveredItem.count;
            if (remainingSpace >= heldItem.count) {
                hoveredItem.count += heldItem.count;
                hoveredItem.RefreshCount();
                Destroy(heldItem.gameObject); 
            }else {
                heldItem.count -= remainingSpace;
                hoveredItem.count = inventoryManager.stackSize;
                hoveredItem.RefreshCount();
                heldItem.RefreshCount();
            }
        }
    }

    // Returns the item that is in this slot
    public InventoryItem GetItemInSlot()
    {
        foreach (Transform child in transform) {
            InventoryItem item = child.GetComponent<InventoryItem>();
            if (item != null) {
                return item;
            }
        }
        return null;
    }

}
