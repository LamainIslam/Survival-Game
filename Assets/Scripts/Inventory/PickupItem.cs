using System.Collections;
using UnityEngine;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public float maxDistance = 2.5f;
    private Camera mainCamera;

    public GameObject itemNameCanvasPrefab;
    private GameObject activeCanvas;
    private GameObject lastHitItem;

    private InventoryManager inventoryManager;
    private HeldItem heldItemComponent;

    private RaycastHit hit;

    void Start()
    {
        // Assign variables
        mainCamera = Camera.main;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        heldItemComponent = GameObject.Find("HeldItem").GetComponent<HeldItem>();
    }

    void Update()
    {
        DisplayItemInfo();
    }

    // Display item information without handling input for pickup
    void DisplayItemInfo()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                // Show canvas and update item name if a different item is hit
                ShowItemNameCanvas(hit);
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

    // Show item name canvas
    void ShowItemNameCanvas(RaycastHit hit)
    {
        if (lastHitItem != hit.collider.gameObject)
        {
            DestroyActiveCanvas();

            activeCanvas = Instantiate(itemNameCanvasPrefab, hit.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            activeCanvas.transform.SetParent(hit.transform);
            TMP_Text itemText = activeCanvas.transform.GetChild(0).GetComponent<TMP_Text>();
            itemText.text = hit.collider.gameObject.name;

            lastHitItem = hit.collider.gameObject;
        }

        // Make the canvas face the camera
        activeCanvas.transform.position = hit.transform.position + Vector3.up * 0.5f;
        Vector3 direction = (mainCamera.transform.position - activeCanvas.transform.position).normalized;
        activeCanvas.transform.rotation = Quaternion.LookRotation(-direction);
    }

    // Handle item pickup logic (called externally by PlayerInputHandler)
    public void TryPickupItem()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.CompareTag("Item"))
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
    }

    // Destroys the active canvas
    void DestroyActiveCanvas()
    {
        if (activeCanvas != null)
        {
            Destroy(activeCanvas);
            lastHitItem = null;
        }
    }
}
