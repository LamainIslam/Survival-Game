using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Item item;

    void Start() {
        // Initialise object name to the item name
        name = item.itemName;
    }
}
