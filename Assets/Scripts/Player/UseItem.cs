using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    private InventoryManager inventoryManager;

    void Start()
    {
        // Assign inventoryManager
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    // Attempts to use selected item
    public void TryUseItem()
    {
        // Assign variables
        Item usedItem = inventoryManager.GetSelectedItem();
        RaycastHit hit = GetComponent<DisplayObjectInfo>().hit;
        float enemyMaxDistance = GetComponent<DisplayObjectInfo>().enemyMaxDistance;
        float resourceMaxDistance = GetComponent<DisplayObjectInfo>().resourceMaxDistance;

        // Find what type of item is being used and do correct action
        if (usedItem != null) {
            if (usedItem.actionType == ActionType.Attack) {
                if (usedItem.type == ItemType.Pickaxe) {
                    if (hit.collider.CompareTag("Rock")&& hit.distance <= resourceMaxDistance) {
                        // Pickaxe does 2x damage to rocks
                        hit.collider.GetComponent<Resource>().health -= usedItem.damagePoints * 2;
                    }
                } else if (usedItem.type == ItemType.Axe) {
                    if (hit.collider.CompareTag("Tree") && hit.distance <= resourceMaxDistance) {
                        // Axe does 2x damage to trees
                        hit.collider.GetComponent<Resource>().health -= usedItem.damagePoints * 2;
                    }
                }
                if (hit.collider.CompareTag("Enemy") && hit.distance <= enemyMaxDistance) {
                    // Weapons damage enemies
                    var hostileScript = hit.collider.GetComponent<AIScriptHostile>();
                    if (hostileScript != null) {
                        hostileScript.TakeDamage(usedItem.damagePoints);
                        return;
                    }
                    var neutralScript = hit.collider.GetComponent<AIScriptNeutral>();
                    if (neutralScript != null) {
                        neutralScript.TakeDamage(usedItem.damagePoints);
                        return;
                    }
                    var passiveScript = hit.collider.GetComponent<AIScriptPassive>();
                    if (passiveScript != null) {
                        passiveScript.TakeDamage(usedItem.damagePoints);
                    }
                }
            } else if (usedItem.actionType == ActionType.Equip) {
                // Armour can be equipped
                GameObject toolbar = GameObject.Find("Toolbar");
                if (usedItem.type == ItemType.Helmet) {
                    toolbar.transform.GetChild(inventoryManager.selectedSlot).transform.GetChild(0).transform.SetParent(inventoryManager.armourSlots[0].transform);
                }else if (usedItem.type == ItemType.Chestplate) {
                    toolbar.transform.GetChild(inventoryManager.selectedSlot).transform.GetChild(0).transform.SetParent(inventoryManager.armourSlots[1].transform);
                }else if (usedItem.type == ItemType.Leggings) {
                    toolbar.transform.GetChild(inventoryManager.selectedSlot).transform.GetChild(0).transform.SetParent(inventoryManager.armourSlots[2].transform);
                }else {
                    toolbar.transform.GetChild(inventoryManager.selectedSlot).transform.GetChild(0).transform.SetParent(inventoryManager.armourSlots[3].transform);
                }
                inventoryManager.UpdateHeldItem();
                Player player = GameObject.Find("Player").GetComponent<Player>();
                player.UpdateDefence();
            } else if(usedItem.actionType == ActionType.Eat) {
                // Food increases hunger
                Player player = GameObject.Find("Player").GetComponent<Player>();
                player.IncreaseHunger(usedItem.hungerRestored);
                inventoryManager.ConsumeSelectedItem();
            } else {
                // Anything else (e.g. resources) do nothing
                Debug.Log("Do Nothing");
            }
        } else {
            // Damage enemy when unarmed
            if (hit.collider.CompareTag("Enemy") && hit.distance <= enemyMaxDistance) {
                var hostileScript = hit.collider.GetComponent<AIScriptHostile>();
                if (hostileScript != null) {
                    hostileScript.TakeDamage(inventoryManager.punchDamage);
                    return;
                }
                var neutralScript = hit.collider.GetComponent<AIScriptNeutral>();
                if (neutralScript != null) {
                    neutralScript.TakeDamage(inventoryManager.punchDamage);
                    return;
                }
                var passiveScript = hit.collider.GetComponent<AIScriptPassive>();
                if (passiveScript != null) {
                    passiveScript.TakeDamage(inventoryManager.punchDamage);
                }
            }
        }
    }
}
