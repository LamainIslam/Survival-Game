using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> listOfItems= new List<Item>();
    private Dictionary<string, Item> itemDictionary; // Dictionary for quick access by item name

    void Awake()
    {
        // Initialize the dictionary
        itemDictionary = new Dictionary<string, Item>();
        foreach (var item in listOfItems)
        {
            itemDictionary[item.itemName] = item; // Use item name as the key
        }
    }

    // Method to get an item by its name
    public Item GetItemByName(string itemName)
    {
        if (itemDictionary.TryGetValue(itemName, out Item foundItem))
        {
            return foundItem; // Return the item if found
        }
        Debug.LogWarning($"Item '{itemName}' not found.");
        return null; // Return null if the item is not found
    }

}