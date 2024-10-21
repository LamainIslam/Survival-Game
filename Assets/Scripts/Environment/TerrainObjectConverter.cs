#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TerrainObjectConverter : MonoBehaviour
{
    public Terrain terrain; // Reference to the terrain
    public GameObject[] objectPrefabs; // Prefabs for trees and rocks, matched by prototype index

    // For editor-time conversion
#if UNITY_EDITOR
    [ContextMenu("Convert Terrain Objects to GameObjects")]
    void ConvertTerrainObjectsToGameObjectsEditor()
    {
        ConvertTerrainObjectsToGameObjects();
    }
#endif

    void ConvertTerrainObjectsToGameObjects()
    {
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
        Vector3 terrainSize = terrain.terrainData.size;

        Debug.Log($"Found {treeInstances.Length} tree instances.");

        // Find the Resources GameObject
        GameObject resourcesParent = GameObject.Find("Resources");
        if (resourcesParent == null)
        {
            Debug.LogError("Resources GameObject not found. Please ensure it exists in the scene.");
            return;
        }

        for (int i = 0; i < treeInstances.Length; i++)
        {
            TreeInstance treeInstance = treeInstances[i];
            Vector3 worldPosition = Vector3.Scale(treeInstance.position, terrainSize) + terrain.transform.position;
            int prototypeIndex = treeInstance.prototypeIndex;

            Debug.Log($"Processing instance {i}: prototype index {prototypeIndex}");

            // Ensure prototype index is valid
            if (prototypeIndex >= 0 && prototypeIndex < objectPrefabs.Length)
            {
                // Instantiate the correct prefab at the tree/rock position
                GameObject newObject = PrefabUtility.InstantiatePrefab(objectPrefabs[prototypeIndex]) as GameObject;

                if (newObject != null)
                {
                    newObject.transform.position = worldPosition;
                    newObject.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    // Set the new object as a child of the Resources GameObject
                    newObject.transform.SetParent(resourcesParent.transform);

                    // Log for debugging
                    Debug.Log($"Instantiated {newObject.name} at {worldPosition} and set as child of {resourcesParent.name}");
                }
                else
                {
                    Debug.LogError($"Failed to instantiate prefab for index {prototypeIndex}");
                }
            }
            else
            {
                Debug.LogError($"Prototype index {prototypeIndex} is out of bounds for prefab array.");
            }
        }

        // Clear tree instances after conversion
        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain.Flush(); // Update the terrain
        Debug.Log("Cleared tree instances from terrain.");
    }
}
