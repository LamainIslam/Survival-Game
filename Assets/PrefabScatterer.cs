using UnityEngine;

public class PrefabScattererWithRaycast : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab; // The prefab to scatter
    public int objectCount = 10; // Number of objects to scatter

    [Header("Scatter Area")]
    public Vector3 areaSize = new Vector3(10, 0, 10); // Width, height, depth of the area

    [Header("Raycast Settings")]
    public float raycastHeight = 10f; // Height above the ground from where the raycast starts
    public LayerMask groundLayer; // Layer mask for the ground

    void Start()
    {
        ScatterPrefabs();
    }

    void ScatterPrefabs()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned!");
            return;
        }

        Vector3 center = transform.position; // Center of the scatter area

        for (int i = 0; i < objectCount; i++)
        {
            // Random position within the defined area
            Vector3 randomPosition = new Vector3(
                Random.Range(center.x - areaSize.x / 2, center.x + areaSize.x / 2),
                center.y + raycastHeight, // Start raycast from above
                Random.Range(center.z - areaSize.z / 2, center.z + areaSize.z / 2)
            );

            // Perform a raycast downward to find the ground
            if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit, raycastHeight * 2, groundLayer))
            {
                // Instantiate the prefab at the hit position
                Instantiate(prefab, hit.point, Quaternion.identity, transform);
            }
            else
            {
                Debug.LogWarning($"No ground detected at {randomPosition}. Skipping this position.");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the scatter area in the editor
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, areaSize);
    }
}

