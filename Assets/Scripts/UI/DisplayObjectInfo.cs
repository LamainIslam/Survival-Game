using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayObjectInfo : MonoBehaviour
{
    private Camera mainCamera;

    // Prefabs for different object types
    public GameObject itemCanvasPrefab;
    public GameObject enemyCanvasPrefab;
    public GameObject resourceCanvasPrefab;
    public GameObject campfireCanvasPrefab;

    // Max distances for different object types
    public float itemMaxDistance = 2.5f;
    public float enemyMaxDistance = 10f;
    public float resourceMaxDistance = 5f;
    public float campfireMaxDistance = 5f;

    private GameObject activeCanvas;
    private GameObject lastHitObject;

    // Public raycast 'hit' to be used by other scripts
    public RaycastHit hit;

    void Start()
    {
        // Assign the main camera
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Constantly try to run the function
        DetectAndDisplayObjectInfo();
    }

    // Detect objects and display appropriate information
    void DetectAndDisplayObjectInfo()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Cast the ray and check if it hits anything within the appropriate distance for each tag
        if (Physics.Raycast(ray, out hit)) {
            // Handle object based on tag and check if it's within the respective max distance
            if (hit.collider.CompareTag("Item") && hit.distance <= itemMaxDistance) {
                ShowCanvas(hit, itemCanvasPrefab, "Item");
            } else if (hit.collider.CompareTag("Enemy") && hit.distance <= enemyMaxDistance) {
                ShowCanvas(hit, enemyCanvasPrefab, "Enemy");
            } else if (hit.collider.CompareTag("Tree") && hit.distance <= resourceMaxDistance) {
                ShowCanvas(hit, resourceCanvasPrefab, "Tree");
            } else if (hit.collider.CompareTag("Rock")&& hit.distance <= resourceMaxDistance) {
                ShowCanvas(hit, resourceCanvasPrefab, "Rock");
            } else if (hit.collider.CompareTag("Campfire")&& hit.distance <= campfireMaxDistance) {
                ShowCanvas(hit, campfireCanvasPrefab, "Campfire");
            } else {
                DestroyActiveCanvas();
            }
        } else {
            DestroyActiveCanvas();
        }
    }

    // Show canvas based on object type
    void ShowCanvas(RaycastHit hit, GameObject canvasPrefab, string type)
    {
        if (lastHitObject != hit.collider.gameObject) {
            DestroyActiveCanvas();
            lastHitObject = hit.collider.gameObject;

            // Instantiate the canvas at the object's position
            activeCanvas = Instantiate(canvasPrefab, hit.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            activeCanvas.transform.SetParent(hit.transform);

            // Change canvas first text to object name
            activeCanvas.transform.GetChild(0).GetComponent<TMP_Text>().text = lastHitObject.name;

            if (type == "Tree" || type == "Rock") {
                Slider slider = activeCanvas.transform.GetChild(1).GetComponent<Slider>();
                slider.maxValue = lastHitObject.GetComponent<Resource>().maxHealth;
                slider.value = lastHitObject.GetComponent<Resource>().health;
            }
        }
        // Make the canvas face the camera
        activeCanvas.transform.position = hit.transform.position + Vector3.up * 0.5f;
        Vector3 direction = (mainCamera.transform.position - activeCanvas.transform.position).normalized;
        activeCanvas.transform.rotation = Quaternion.LookRotation(-direction);
    }

    // Destroys the active canvas
    void DestroyActiveCanvas()
    {
        if (activeCanvas != null) {
            Destroy(activeCanvas);
            lastHitObject = null;
        }
    }
}
