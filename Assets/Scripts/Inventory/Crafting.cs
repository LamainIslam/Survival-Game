using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crafting : MonoBehaviour
{
    public List<InventorySlot> craftingSlots = new List<InventorySlot>();
    //public List<string[,]> recipe = new List<string[,]>();
    //public List<(string item, int quantity)>[] recipe = new List<(string item, int quantity)>[];
    public List<List<(string item, int quantity)>> recipes = new List<List<(string item, int quantity)>>();
    public InventorySlot outputSlot;
    public Button left;
    public Button right;
    public Button craft;
    public TextMeshProUGUI requirements;
    public InventoryItem itemPrefab;  // Reference to the InventoryItem prefab


    private int currentRecipeIndex = 0;

    void Start()
    {
        AddRecipe("Stone", 1, "Wood", 2, "", 0, "Axe", 1);
        AddRecipe("Stone", 2, "Wood", 1, "", 0, "Pickaxe", 1);

        left.onClick.AddListener(PreviousRecipe);
        right.onClick.AddListener(NextRecipe);
        craft.onClick.AddListener(CraftItem);

        // Show the first recipe's requirements
        DisplayCurrentRecipe();
    }

    void AddRecipe(string item1, int quantity1, string item2, int quantity2, string item3, int quantity3, string item4, int quantity4)
    {
        var newRecipe = new List<(string item, int quantity)>
        {
            (item1, quantity1),
            (item2, quantity2),
            (item3, quantity3),
            (item4, quantity4)
        };

        // Optionally filter out empty items
        //newRecipe.RemoveAll(x => string.IsNullOrEmpty(x.item) || x.quantity <= 0);

        recipes.Add(newRecipe);
    }

    void PreviousRecipe()
    {
        currentRecipeIndex = (currentRecipeIndex - 1 + recipes.Count) % recipes.Count;
        DisplayCurrentRecipe();
    }

    void NextRecipe(){
        currentRecipeIndex = (currentRecipeIndex + 1) % recipes.Count;
        DisplayCurrentRecipe();
    }

    void DisplayCurrentRecipe(){
        var recipe = recipes[currentRecipeIndex];
        string requirementText = "";
        string outputItem = $"{recipe[recipe.Count-1].item}";

        requirementText += $"{outputItem}:\n";
        
        for (int i = 0; i < recipe.Count - 1; i++) // Exclude the result item
        {
            if (!string.IsNullOrEmpty(recipe[i].item))
            {
                requirementText += $"{recipe[i].quantity}x {recipe[i].item}\n";
            }
        }

        requirements.text = requirementText;
    }

    void CraftItem()
    {
        var currentRecipe = recipes[currentRecipeIndex];

        // Extract the output (last element of the recipe list)
        var result = currentRecipe[currentRecipe.Count - 1];

        // Create a dictionary to store required input items
        Dictionary<string, int> requiredItems = new Dictionary<string, int>();

        // Collect all input items (all elements except the last one)
        for (int i = 0; i < currentRecipe.Count - 1; i++)
        {
            var ingredient = currentRecipe[i];
            if (!string.IsNullOrEmpty(ingredient.item) && ingredient.quantity > 0)
            {
                if (requiredItems.ContainsKey(ingredient.item))
                    requiredItems[ingredient.item] += ingredient.quantity;
                else
                    requiredItems[ingredient.item] = ingredient.quantity;
            }
        }

        // Check if the crafting slots contain the required items
        bool canCraft = true;
        Dictionary<string, int> itemsInSlots = new Dictionary<string, int>();

        foreach (var slot in craftingSlots)
        {
            var item = slot.GetItemInSlot();
            if (item != null)
            {
                if (itemsInSlots.ContainsKey(item.item.itemName))
                    itemsInSlots[item.item.itemName] += item.count;
                else
                    itemsInSlots[item.item.itemName] = item.count;
            }
        }

        // Validate required items and quantity
        foreach (var requiredItem in requiredItems)
        {
            Debug.Log($"Checking required item: {requiredItem.Key}, needed: {requiredItem.Value}, available: {(itemsInSlots.ContainsKey(requiredItem.Key) ? itemsInSlots[requiredItem.Key].ToString() : "0")}");


            if (!itemsInSlots.ContainsKey(requiredItem.Key) ||
                itemsInSlots[requiredItem.Key] < requiredItem.Value)
            {
                canCraft = false;
                break;
            }
        }

        if (canCraft)
        {
            // Get the ItemManager
            ItemManager itemManager = FindAnyObjectByType<ItemManager>();
            if (itemManager == null)
            {
                Debug.LogError("ItemManager not found.");
                return; // Exit if the ItemManager is not found
            }

            // Retrieve the crafted item from the ItemManager
            Item craftedItem = itemManager.GetItemByName(result.item);
            if (craftedItem == null)
            {
                Debug.LogError($"Crafted item '{result.item}' does not exist in ItemManager.");
                return; // Exit if the item does not exist
            }

            // Instantiate the crafted item in the output slot
            GameObject newItemObject = Instantiate(itemPrefab.gameObject, outputSlot.transform);
            InventoryItem newInventoryItem = newItemObject.GetComponent<InventoryItem>();

            // Initialise the new item with the output data
            newInventoryItem.InitialiseItem(craftedItem);
            newInventoryItem.count = result.quantity;
            newInventoryItem.RefreshCount();

            Debug.Log($"Crafted: {result.quantity}x {result.item}");

            //Reduce the items from the crafting slots
            ReduceItemsInSlots(requiredItems);
        }
        else
        {
            Debug.Log("Invalid recipe or missing items.");
        }
    }


    void ReduceItemsInSlots(Dictionary<string, int> requiredItems)
    {
        foreach (var slot in craftingSlots)
        {
            var item = slot.GetItemInSlot();
            if (item != null && requiredItems.ContainsKey(item.item.itemName))
            {
                int reduceAmount = Mathf.Min(requiredItems[item.item.itemName], item.count);
                item.count -= reduceAmount;
                requiredItems[item.item.itemName] -= reduceAmount;

                if (item.count <= 0)
                    Destroy(item.gameObject);
                else
                    item.RefreshCount();
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
