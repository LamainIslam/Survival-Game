using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractWithItem : MonoBehaviour
{
    public float maxDistance = 2.5f;
    private Camera mainCamera;
    private InventoryManager inventoryManager;
    private HeldItem heldItemComponent;
    private RaycastHit hit;

    public GameObject[] mainInventoryTop;

    void Start()
    {
        // Assign variables
        mainCamera = Camera.main;
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        heldItemComponent = GameObject.Find("HeldItem").GetComponent<HeldItem>();
    }

    // Handle item interaction logic (called externally by PlayerInputHandler)
    public void TryInteractWithItem()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.CompareTag("Item")) {
            // Add item to inventory and update held item
            var itemHolder = hit.collider.gameObject.GetComponent<ItemHolder>();
            bool canAddItem = inventoryManager.AddItem(itemHolder.item);
            if(canAddItem == true) {
                heldItemComponent.heldItem = inventoryManager.GetSelectedItem();
                heldItemComponent.HoldItem(heldItemComponent.heldItem);

                // Destroy item
                Destroy(hit.collider.gameObject);
            }
        }else if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.CompareTag("Campfire")) {
            // Open campfire internal UI if it doesn't already exist
            GameObject CampfireInternalUI = GameObject.Find("CampfireInternalCanvas");  
            if(CampfireInternalUI == null){
                hit.collider.GetComponent<Campfire>().OpenUI();
                
                inventoryManager.ToggleInventory();
                for (int i = 0; i < mainInventoryTop.Length; i++) {
                    mainInventoryTop[i].SetActive(false);
                }
            }
        }
    }
}
