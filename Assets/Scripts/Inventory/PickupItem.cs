using System.Collections;
using UnityEngine;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public float maxDistance = 2.5f;
    private Camera mainCamera;

    public GameObject itemNameCanvasPrefab;
    private GameObject activeCanvas;
    private GameObject lastHitItem; // Store reference to the last hit item

    private InventoryManager inventoryManager;
    private HeldItem heldItemComponent;

    void Start()
    {
        // Cache references to camera, inventory manager, and held item
        mainCamera = Camera.main;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        heldItemComponent = GameObject.Find("HeldItem").GetComponent<HeldItem>();
    }

    void Update()
    {
        HandleItemPickup();
    }

    void HandleItemPickup()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                // Show canvas and update item name if a different item is hit
                ShowItemNameCanvas(hit);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupAndDestroyItem(hit);
                }
            }
            else
            {
                DestroyActiveCanvas();
            }
        }
        else
        {
            DestroyActiveCanvas();
        }
    }

    void ShowItemNameCanvas(RaycastHit hit)
    {
        // If the hit item is different from the last hit item
        if (lastHitItem != hit.collider.gameObject)
        {
            DestroyActiveCanvas();  // Destroy old canvas

            // Instantiate canvas and set item name for the new hit item
            activeCanvas = Instantiate(itemNameCanvasPrefab, hit.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            activeCanvas.transform.SetParent(hit.transform);
            TMP_Text itemText = activeCanvas.transform.GetChild(0).GetComponent<TMP_Text>();
            itemText.text = hit.collider.gameObject.name;

            // Update the reference to the current item
            lastHitItem = hit.collider.gameObject;
        }

        // Make the canvas face the camera
        activeCanvas.transform.position = hit.transform.position + Vector3.up * 0.5f;
        Vector3 direction = (mainCamera.transform.position - activeCanvas.transform.position).normalized;
        activeCanvas.transform.rotation = Quaternion.LookRotation(-direction);
    }

    void PickupAndDestroyItem(RaycastHit hit)
    {
        // Add item to inventory and update held item
        var itemHolder = hit.collider.gameObject.GetComponent<ItemHolder>();
        inventoryManager.AddItem(itemHolder.item);
        heldItemComponent.heldItem = inventoryManager.GetSelectedItem(false);
        heldItemComponent.HoldItem(heldItemComponent.heldItem);

        // Destroy item and canvas
        Destroy(hit.collider.gameObject);
        Destroy(activeCanvas);
    }

    void DestroyActiveCanvas()
    {
        if (activeCanvas != null)
        {
            Destroy(activeCanvas);
            lastHitItem = null;  // Reset the last hit item reference
        }
    }
}
