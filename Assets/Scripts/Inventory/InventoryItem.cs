using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public TMP_Text countText;
    public Item item;
    public int count = 1;
    public Transform parentAfterDrag;

    public InventoryItem Clone()
    {
        GameObject cloneObject = Instantiate(this.gameObject);
        InventoryItem clone = cloneObject.GetComponent<InventoryItem>();

        // Copy data
        clone.item = this.item;
        clone.count = this.count;
        clone.name = this.name;

        return clone;
    }

    // Initialises item
    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
        gameObject.name = item.itemName;
    }

    // Changes the count of the items, hides count if only 1
    public void RefreshCount()
    {
        countText.text = count.ToString();
        if (count > 1) {
            countText.gameObject.SetActive(true);
        }else {
            countText.gameObject.SetActive(false);
        }
    }

    // Finds item under cursor
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        // Split stack if using right mouse button and more than 1 item
        if (eventData.button == PointerEventData.InputButton.Right && count > 1)
        {
            SplitStack();
        }
    }

    // Moves item with mouse
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    // Drops item where mouse is released
    public void OnEndDrag(PointerEventData eventData)
    {
        // Set items parent to new slot
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        // Update held item
        InventoryManager inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        inventoryManager.UpdateHeldItem();

        // Update defence
        Player player = GameObject.Find("Player").GetComponent<Player>();
        player.UpdateDefence();
    }

    // Function for splitting the stack
    private void SplitStack()
    {
        int splitCount = Mathf.FloorToInt(count / 2);

        // Create a new item for the split stack
        GameObject newItemObject = Instantiate(gameObject, transform.parent);
        InventoryItem newInventoryItem = newItemObject.GetComponent<InventoryItem>();

        // Initialise the new item with the same data but half the count
        newInventoryItem.InitialiseItem(item);
        newInventoryItem.count = splitCount;
        newInventoryItem.RefreshCount();
        newInventoryItem.image.raycastTarget = true;

        // Update the current item's count
        count -= splitCount;
        RefreshCount();

        // Set the new items parent to the original parent
        newInventoryItem.parentAfterDrag = parentAfterDrag;
        newInventoryItem.transform.SetParent(parentAfterDrag);

        // Update held item
        InventoryManager inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        inventoryManager.UpdateHeldItem();
    }

}