using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int stackSize = 10;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    
    int selectedSlot = -1;

    public Item empty;

    private void Start() {
        ChangeSelectedSlot(0);
    }

    private void Update() {
        // Use number row to change toobar item selection
        if (Input.inputString != null) {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number >= 1 && number <= 8) {
                ChangeSelectedSlot(number - 1);
            }else if (isNumber && number == 0) {
                ChangeSelectedSlot(7);
            }
        }

        // Use scroll wheel to change toobar item selection
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (selectedSlot < 7) {
                ChangeSelectedSlot(selectedSlot + 1);
            }else {
                ChangeSelectedSlot(0);
            }
        }else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (selectedSlot > 0) {
                ChangeSelectedSlot(selectedSlot - 1);
            }else {
                ChangeSelectedSlot(7);
            }
        }
    }

    // Changes selected slot
    public void ChangeSelectedSlot(int newValue) {
        if (selectedSlot >= 0) {
            inventorySlots[selectedSlot].Deselect();
        }

        inventorySlots[newValue].Select();
        selectedSlot = newValue;

        // Display held item
        if (inventorySlots[selectedSlot].transform.childCount > 0) {
            GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem = inventorySlots[selectedSlot].transform.GetChild(0).GetComponent<InventoryItem>().item;
            GameObject.Find("HeldItem").GetComponent<HeldItem>().HoldItem(GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem);
        }else {
            GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem = empty;
            GameObject.Find("HeldItem").GetComponent<HeldItem>().HoldItem(null);
        }
    }

    public bool AddItem(Item item) {
        // Check if possible to add to an existing stack
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < stackSize && itemInSlot.item.stackable == true) {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        // Find the next empty slot
        for (int i = 0; i < inventorySlots.Length; i++) {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null) {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }

    // Spawns item into inventory
    void SpawnNewItem(Item item, InventorySlot slot) {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    // Returns selected item and decreases count by 1 if use == true. Returns null if empty
    public Item GetSelectedItem(bool use) {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) {
            Item item = itemInSlot.item;
            if (use == true) {
                itemInSlot.count--;
                if (itemInSlot.count <= 0) {
                    Destroy(itemInSlot.gameObject);
                }else {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }

    public void UpdateHeldItem()
    {
        // Display the new held item based on the currently selected slot
        if (inventorySlots[selectedSlot].transform.childCount > 0)
        {
            GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem = inventorySlots[selectedSlot].transform.GetChild(0).GetComponent<InventoryItem>().item;
            GameObject.Find("HeldItem").GetComponent<HeldItem>().HoldItem(GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem);
        }
        else
        {
            GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem = empty;
            GameObject.Find("HeldItem").GetComponent<HeldItem>().HoldItem(null);
        }
    }
}
