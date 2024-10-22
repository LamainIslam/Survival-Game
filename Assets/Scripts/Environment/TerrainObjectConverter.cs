#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TerrainObjectConverter : MonoBehaviour
{
    public Terrain terrain;
    public GameObject[] objectPrefabs;

    // Enables ability to run function from editor
#if UNITY_EDITOR
    [ContextMenu("Convert Terrain Objects to GameObjects")]
    void ConvertTerrainObjectsToGameObjectsEditor()
    {
        ConvertTerrainObjectsToGameObjects();
    }
#endif

    // Converts terrain objects (trees and rocks) to game objects
    void ConvertTerrainObjectsToGameObjects()
    {
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
        Vector3 terrainSize = terrain.terrainData.size;

        // Find the Resources GameObject
        GameObject resourcesParent = GameObject.Find("Resources");
        if (resourcesParent == null) {
            return;
        }

        for (int i = 0; i < treeInstances.Length; i++) {
            TreeInstance treeInstance = treeInstances[i];
            Vector3 worldPosition = Vector3.Scale(treeInstance.position, terrainSize) + terrain.transform.position;
            int prototypeIndex = treeInstance.prototypeIndex;

            // Instantiate the correct prefab at the tree/rock position if prototype index is valid
            if (prototypeIndex >= 0 && prototypeIndex < objectPrefabs.Length) {
                GameObject newObject = PrefabUtility.InstantiatePrefab(objectPrefabs[prototypeIndex]) as GameObject;
                if (newObject != null) {
                    newObject.transform.position = worldPosition;
                    newObject.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    newObject.transform.SetParent(resourcesParent.transform);
                }
            }
        }
        // Clear tree instances after conversion
        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain.Flush();
    }
}
