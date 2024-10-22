using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public TMP_Text countText;


    [HideInInspector] public Item item;
    public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

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
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    // Finds item under cursor
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        // Split stack if more than 1 item
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
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);

        // Update held item
        InventoryManager inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        inventoryManager.UpdateHeldItem();
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

        // Set the parent of the new item to be the original parent
        newInventoryItem.parentAfterDrag = parentAfterDrag;
        newInventoryItem.transform.SetParent(parentAfterDrag);

        // Update held item
        InventoryManager inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        inventoryManager.UpdateHeldItem();
    }
}