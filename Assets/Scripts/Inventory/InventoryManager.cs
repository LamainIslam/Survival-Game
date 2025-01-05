using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int stackSize = 10;
    public InventorySlot[] inventorySlots;
    public GameObject[] armourSlots;
    public InventorySlot offHandSlot;
    public GameObject inventoryItemPrefab;
    public GameObject crossHair;
    public GameObject mainInventoryGroup;
    public int selectedSlot = -1;
    public Item empty;
    public float punchDamage;
    public bool inventoryOn;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
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
    public void ChangeSelectedSlot(int newValue)
    {
        // Sets slot colours
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

    public bool AddItem(Item item)
    {
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

    // Drops held item
    public void DropItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        // Check if there is an item in the selected slot to drop
        if (itemInSlot != null)
        {
            // Get the item details
            Item item = itemInSlot.item;

            // Remove the item from the inventory
            Destroy(itemInSlot.gameObject);

            // Spawn the item in the world
            SpawnDroppedItem(item);
            
            // Update the held item in the inventory UI
            StartCoroutine(UpdateHeldItemNextFrame());
        }
    }

    // Spawns the dropped item in front of the player
    void SpawnDroppedItem(Item item)
    {
        // Find the held item GameObject
        GameObject hand = GameObject.Find("HeldItem");
        GameObject heldItemTransform = hand.transform.GetChild(0).gameObject;

        if (heldItemTransform != null)
        {
            // Destroy the held item in hand
            Destroy(heldItemTransform);

            // Instantiate the item prefab at a position in front of the player
            GameObject droppedItem = Instantiate(item.prefab, hand.transform.position, Quaternion.identity);

            // Add Rigidbody and BoxCollider components
            Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
            if (rb == null) rb = droppedItem.AddComponent<Rigidbody>();
            if (droppedItem.GetComponent<BoxCollider>() == null) droppedItem.AddComponent<BoxCollider>();

            // Calculate force based on player's velocity
            Vector3 playerVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().velocity;
            Vector3 initialForce = hand.transform.forward * 10f + playerVelocity;

            // Apply force to throw the item forward, adjusted by player speed
            rb.AddForce(initialForce, ForceMode.Impulse);
        }
    }

    // Spawns item into inventory
    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    // Returns selected item
    public Item GetSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) {
            Item item = itemInSlot.item;
            return item;
        }
        return null;
    }

    // Decreases count of selected item by 1
    public void ConsumeSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) {
            Item item = itemInSlot.item;
            itemInSlot.count--;
            if (itemInSlot.count <= 0) {
                Destroy(itemInSlot.gameObject);
            }else {
                itemInSlot.RefreshCount();
            }
        }
        StartCoroutine(UpdateHeldItemNextFrame());
    }

    // Returns selected item
    public Item GetOffHandItem()
    {
        InventorySlot slot = offHandSlot;
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) {
            Item item = itemInSlot.item;
            Debug.Log(itemInSlot.item);
            return item;
        }
        return null;
    }

    // Decreases count of selected item by 1
    public void ConsumeOffHandItem()
    {
        InventorySlot slot = offHandSlot;
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) {
            Item item = itemInSlot.item;
            itemInSlot.count--;
            if (itemInSlot.count <= 0) {
                Destroy(itemInSlot.gameObject);
            }else {
                itemInSlot.RefreshCount();
            }
        }
        StartCoroutine(UpdateHeldItemNextFrame());
    }

    // Updates the held item to the current held item
    public void UpdateHeldItem()
    {
        // Display the new held item based on the currently selected slot
        if (inventorySlots[selectedSlot].transform.childCount > 0) {
            GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem = inventorySlots[selectedSlot].transform.GetChild(0).GetComponent<InventoryItem>().item;
            GameObject.Find("HeldItem").GetComponent<HeldItem>().HoldItem(GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem);
        }else {
            GameObject.Find("HeldItem").GetComponent<HeldItem>().heldItem = empty;
            GameObject.Find("HeldItem").GetComponent<HeldItem>().HoldItem(null);
        }
        // Repeat same thing for offhand item
        if (offHandSlot.transform.childCount > 0) {
            GameObject.Find("OffHandHeldItem").GetComponent<HeldItem>().heldItem = offHandSlot.transform.GetChild(0).GetComponent<InventoryItem>().item;
            GameObject.Find("OffHandHeldItem").GetComponent<HeldItem>().HoldItem(GameObject.Find("OffHandHeldItem").GetComponent<HeldItem>().heldItem);
        }else {
            GameObject.Find("OffHandHeldItem").GetComponent<HeldItem>().heldItem = empty;
            GameObject.Find("OffHandHeldItem").GetComponent<HeldItem>().HoldItem(null);
        }
    }

    // Update held item on the next frame
    private IEnumerator UpdateHeldItemNextFrame()
    {
        yield return null;
        UpdateHeldItem();
    }

    // Toggles main inventory
    public void ToggleInventory()
    {
        if (mainInventoryGroup.activeInHierarchy == false) {
            inventoryOn = true;
            mainInventoryGroup.SetActive(true);
            crossHair.SetActive(false);
            GameObject.Find("PlayerCameraHolder").transform.GetChild(0).GetComponent<PlayerCamera>().lockCursor = false;
        }else {
            inventoryOn = false;
            for (int i = 0; i < mainInventoryGroup.transform.childCount; i++) {
                mainInventoryGroup.transform.GetChild(i).gameObject.SetActive(true);
            }
            mainInventoryGroup.SetActive(false);
            crossHair.SetActive(true);
            GameObject.Find("PlayerCameraHolder").transform.GetChild(0).GetComponent<PlayerCamera>().lockCursor = true;
            // Delete campfire UI if it exists
            GameObject CampfireInternalUI = GameObject.Find("CampfireInternalCanvas");  
            if(CampfireInternalUI != null){
                Destroy(CampfireInternalUI);
            }
        }
    }
}
