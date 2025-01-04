using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour
{
    public GameObject InternalUIPrefab;
    public GameObject internalUI;

    public InventoryItem fuel; // Slot for fuel items
    public InventoryItem input; // Slot for raw food
    public InventoryItem output; // Slot for cooked food

    public float fuelAmount; // Current fuel level
    public float fuelDecreaseRate = 1f; // Rate at which fuel decreases over time
    public float cookingSpeed = 1f; // Speed at which food cooks
    public float coolingSpeed = 0.25f; // Speed at which cooking progress decreases when no fuel

    public Slider fuelSlider; // UI slider for fuel amount
    public Slider cookedSlider; // UI slider for cooking progress

    private bool isBurning = false;
    private float cookedAmount = 0f; // Current cooking progress
    private float maxCookedAmount = 5f; // Maximum cooking progress

    public InventoryManager inventoryManager;

    public GameObject[] cookedFoodPrefabs;

    void Start()
    {
        UpdateFuelUI();
        UpdateCookedUI();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    void Update()
    {
        if (internalUI != null) {
            if(internalUI.transform.GetChild(0).GetChild(0).transform.childCount > 0) {
                fuel = internalUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<InventoryItem>();
            }else{
                fuel = null;
            }
            if(internalUI.transform.GetChild(0).GetChild(1).transform.childCount > 0) {
                input = internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>();
            }else{
                input = null;
            }
            if(internalUI.transform.GetChild(0).GetChild(2).transform.childCount > 0) {
                output = internalUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<InventoryItem>();
            }else{
                output = null;
            }

            fuelSlider = internalUI.transform.GetChild(0).GetChild(3).GetComponent<Slider>();
            cookedSlider = internalUI.transform.GetChild(0).GetChild(4).GetComponent<Slider>();
        }
        

        // Handle fuel burning
        if (fuelAmount > 0)
        {
            fuelAmount -= fuelDecreaseRate * Time.deltaTime;
            UpdateFuelUI();

            if (fuelAmount <= 0)
            {
                fuelAmount = 0;
                isBurning = false;
            }
        }

        // Handle cooking process
        if (fuelAmount > 0 && input != null)
        {
            CookFood();
        }
        else if (cookedAmount > 0)
        {
            CoolDown();
        }

        if (fuelAmount == 0 && fuel != null) // Only add fuel if fuelAmount is 0
        {
            fuelAmount += fuel.item.fuelValue; // Add fuel value
            if (fuel.count > 0) {
                fuel.count--;
                fuel.RefreshCount();
            }else {
                Destroy(input.gameObject);
                isBurning = true; // Start burning
            }
            UpdateFuelUI();
        }
    }

    // Cooks food in the input slot
    private void CookFood()
    {
        cookedAmount += cookingSpeed * Time.deltaTime;

        if (cookedAmount >= maxCookedAmount)
        {
            cookedAmount = 0f;
            ProcessCookedFood();
        }

        UpdateCookedUI();
    }

    // Resets cooking progress when fuel runs out
    private void CoolDown()
    {
        cookedAmount -= coolingSpeed * Time.deltaTime;

        if (cookedAmount < 0)
        {
            cookedAmount = 0;
        }

        UpdateCookedUI();
    }

    // Processes the cooked food
    private void ProcessCookedFood()
    {
        // Get the name of the cooked food
        string cookedFoodName = input.gameObject.name.Replace("Raw", "Cooked");

        // Check if the cooked food exists in the array
        GameObject cookedPrefab = null;
        foreach (GameObject prefab in cookedFoodPrefabs)
        {
            if (prefab.name == cookedFoodName)
            {
                cookedPrefab = prefab;
                break;
            }
        }

        // If the cooked food doesn't exist, return
        if (cookedPrefab == null)
        {
            Debug.LogWarning("No matching cooked food found for: " + cookedFoodName);
            return;
        }

        // Add cooked food to the output slot
        if (output == null)
        {
            GameObject cookedItem = Instantiate(cookedPrefab);
            cookedItem.GetComponent<InventoryItem>().count = 1;
            cookedItem.GetComponent<InventoryItem>().RefreshCount();
            cookedItem.transform.SetParent(internalUI.transform.GetChild(0).GetChild(2).gameObject.transform);
            cookedItem.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            cookedItem.name = cookedItem.name.Replace("(Clone)", "");

            // Reduce raw food count
            if (input.item != null)
            {
                input.count--;
                input.RefreshCount();

                if (input.count <= 0)
                {
                    Destroy(input.gameObject); // Clear the input slot if no items left
                }
            }
        }
        else if (output.name == cookedFoodName && output.count < inventoryManager.stackSize)
        {
            output.count++;
            output.RefreshCount();

            // Reduce raw food count
            if (input.item != null)
            {
                input.count--;
                input.RefreshCount();

                if (input.count <= 0)
                {
                    Destroy(input.gameObject); // Clear the input slot if no items left
                }
            }
        }
        else
        {
            Debug.Log(output.name + cookedFoodName);
        }
    }


    // Updates the fuel slider UI
    private void UpdateFuelUI()
    {
        if (fuelSlider != null)
        {
            fuelSlider.value = fuelAmount;
        }
    }

    // Updates the cooked slider UI
    private void UpdateCookedUI()
    {
        if (cookedSlider != null)
        {
            cookedSlider.value = cookedAmount / maxCookedAmount;
        }
    }

    public void OpenUI()
    {
        internalUI = Instantiate(InternalUIPrefab);
        internalUI.name = "CampfireInternalCanvas";
        internalUI.transform.SetParent(GameObject.Find("Canvas").transform);
    }
}