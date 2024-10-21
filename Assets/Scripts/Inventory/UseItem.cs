using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    private InventoryManager inventoryManager;

    void Start()
    {
        // Assign variables
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    // Uses your item
    public void TryUseItem()
    {
        // Assign variables
        Item usedItem = inventoryManager.GetSelectedItem(false);
        RaycastHit hit = GetComponent<DisplayObjectInfo>().hit;
        float enemyMaxDistance = GetComponent<DisplayObjectInfo>().enemyMaxDistance;
        float resourceMaxDistance = GetComponent<DisplayObjectInfo>().resourceMaxDistance;

        // Find what type of item is being used and do correct action
        if (usedItem != null) {
            if (usedItem.actionType == ActionType.Attack) {
                if (usedItem.type == ItemType.Pickaxe) {
                    if (hit.collider.CompareTag("Rock")&& hit.distance <= resourceMaxDistance) {
                        hit.collider.GetComponent<Resource>().health -= usedItem.damagePoints * 2;
                    }
                } else if (usedItem.type == ItemType.Axe) {
                    if (hit.collider.CompareTag("Tree") && hit.distance <= resourceMaxDistance) {
                        hit.collider.GetComponent<Resource>().health -= usedItem.damagePoints * 2;
                    }
                }
                if (hit.collider.CompareTag("Enemy") && hit.distance <= enemyMaxDistance) {
                    // Link to enemy script with health, decrease as shown in example below
                    // hit.collider.GetComponent<Enemy>().health -= usedItem.damagePoints;
                }
            } else if (usedItem.actionType == ActionType.Equip) {
                Debug.Log("Equip");
            } else if(usedItem.actionType == ActionType.Eat) {
                Debug.Log("Eat");
            } else {
                Debug.Log("Do Nothing");
            }
        } else {
            Debug.Log("Empty");
        }
    }
}
