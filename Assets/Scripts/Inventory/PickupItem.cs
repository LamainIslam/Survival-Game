using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupItem : MonoBehaviour
{
    public float maxDistance = 2.5f;  
    private Camera mainCamera;

    public GameObject worldSpaceCanvasPrefab;
    private GameObject activeCanvas;

    void Start()
    {
        // Initialise camera
        mainCamera = Camera.main;
    }

    void Update() 
    {
        // Create a ray from the center of screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Check if ray hits object with "Item" tag
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                if (activeCanvas == null)
                {
                    // Create the canvas next to the item
                    activeCanvas = Instantiate(worldSpaceCanvasPrefab, hit.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                    activeCanvas.transform.SetParent(hit.transform);
                    
                    TMP_Text itemText = activeCanvas.transform.GetChild(0).GetComponent<TMP_Text>();
                    itemText.text = hit.collider.gameObject.name;
                }
                
                // Make text face camera
                activeCanvas.transform.position = hit.transform.position + Vector3.up * 0.5f;
                Vector3 direction = (mainCamera.transform.position - activeCanvas.transform.position).normalized;
                activeCanvas.transform.rotation = Quaternion.LookRotation(-direction);
                
                // Pick up the item
                if (Input.GetKeyDown(KeyCode.F))
                {
                    GameObject.Find("InventoryManager").GetComponent<InventoryManager>().AddItem(hit.collider.gameObject.GetComponent<ItemHolder>().item); 
                    Destroy(hit.collider.gameObject);
                    Destroy(activeCanvas);
                }
            }
            else
            {
                // Destroy canvas when looking away
                if (activeCanvas != null)
                {
                    Destroy(activeCanvas);
                }
            }
        }
        else
        {
            // Destroy canvas if nothing is hit
            if (activeCanvas != null)
            {
                Destroy(activeCanvas);
            }
        }
    }
}