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

    public float cookedAmount = 0f; // Current cooking progress
    public float maxCookedAmount = 5f; // Maximum cooking progress

    public InventoryManager inventoryManager;

    public GameObject[] cookedFoodPrefabs;

    public GameObject particles;
    public GameObject emptyItem;

    void Start()
    {
        UpdateFuelUI();
        UpdateCookedUI();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        particles.SetActive(false);
    }

    void Update()
    {
        if (internalUI != null) {
            if(internalUI.transform.GetChild(0).GetChild(0).transform.childCount > 0) {
                if (fuel == null) {
                    InventoryItem originalFuel = internalUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<InventoryItem>();
                    fuel = originalFuel.Clone();
                }
            }else{
                fuel = null;
            }
            if(internalUI.transform.GetChild(0).GetChild(1).transform.childCount > 0) {
                if (input == null) {
                    InventoryItem originalInput = internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>();
                    input = originalInput.Clone();
                }
            }else{
                input = null;
            }
            if(internalUI.transform.GetChild(0).GetChild(2).transform.childCount > 0) {
                if (output == null) {
                    InventoryItem originalOutput = internalUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<InventoryItem>();
                    output = originalOutput.Clone();
                }
            }else{
                output = null;
            }
            fuelSlider = internalUI.transform.GetChild(0).GetChild(3).GetComponent<Slider>();
            cookedSlider = internalUI.transform.GetChild(0).GetChild(4).GetComponent<Slider>();

            
            if (internalUI.transform.GetChild(0).GetChild(0).transform.childCount > 0 && internalUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<InventoryItem>() == null) {
                Destroy(fuel.gameObject);
            }
            if (internalUI.transform.GetChild(0).GetChild(1).transform.childCount > 0 && internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>() == null) {
                Destroy(input.gameObject);
            }
            if (internalUI.transform.GetChild(0).GetChild(2).transform.childCount > 0 && internalUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<InventoryItem>() == null) {
                Destroy(output.gameObject);
            }
        }

        // Handle fuel burning
        if (fuelAmount > 0)
        {
            particles.SetActive(true);

            fuelAmount -= fuelDecreaseRate * Time.deltaTime;
            UpdateFuelUI();

            if (fuelAmount <= 0)
            {
                fuelAmount = 0;
            }
        }else{
            particles.SetActive(false);
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
            if (fuel.count > 1) {
                fuel.count--;
                fuel.RefreshCount();
                if (internalUI != null) {
                    internalUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<InventoryItem>().count--;
                    internalUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                }
            }else {
                Destroy(fuel.gameObject);
                if (internalUI != null) {
                    Destroy(internalUI.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
                }
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
            GameObject cookedObject = Instantiate(cookedPrefab);
            cookedObject.name = cookedPrefab.name;
            output = cookedObject.GetComponent<InventoryItem>();


            // Reduce raw food count
            if (input != null)
            {
                input.count--;
                input.RefreshCount();
                if (internalUI != null) {
                    internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>().count--;
                    internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                }

                if (input.count <= 1)
                {
                    Destroy(input.gameObject);
                    if (internalUI != null) {
                        Destroy(internalUI.transform.GetChild(0).GetChild(1).GetChild(0).gameObject);
                    }
                }
            }

            if (output != null)
            {
                // Instantiate the empty item (output slot placeholder)
                if (internalUI != null) {
                    GameObject outputObject = Instantiate(emptyItem);
                    outputObject.transform.SetParent(internalUI.transform.GetChild(0).GetChild(2));
                    outputObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    
                    // Update the InventoryItem component of the instantiated object
                    InventoryItem outputItem = outputObject.GetComponent<InventoryItem>();
                    if (outputItem == null)
                    {
                        outputItem = outputObject.AddComponent<InventoryItem>();
                    }

                    // Copy all information from the output object to the new InventoryItem
                    outputItem.item = output.item;
                    outputItem.count = output.count;
                    outputItem.gameObject.name = output.item.name;
                    outputItem.image = outputItem.GetComponent<Image>();
                    outputItem.image.sprite = output.item.image;
                    outputItem.gameObject.GetComponent<Image>().sprite = output.item.image;
                    outputItem.RefreshCount();
                }
            }
        }
        else if (output.name == cookedFoodName && output.count < inventoryManager.stackSize)
        {
            output.count++;
            output.RefreshCount();
            if (internalUI != null) {
                internalUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<InventoryItem>().count++;
                internalUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<InventoryItem>().RefreshCount();
            }

            // Reduce raw food count
            if (input.item != null)
            {
                input.count--;
                input.RefreshCount();
                if (internalUI != null) {
                    internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>().count--;
                    internalUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<InventoryItem>().RefreshCount();
                }

                if (input.count <= 0)
                {
                    Destroy(input.gameObject);
                    if (internalUI != null) {
                        Destroy(internalUI.transform.GetChild(0).GetChild(1).GetChild(0).gameObject);
                    }
                }
            }
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
        // Instantiate the internal UI
        internalUI = Instantiate(InternalUIPrefab);
        internalUI.name = "CampfireInternalCanvas";
        internalUI.transform.SetParent(GameObject.Find("Canvas").transform);

        // Find the fuel slot (first child of the first child of internalUI)
        Transform fuelSlot = internalUI.transform.GetChild(0).GetChild(0);

        // Check if fuel exists
        if (fuel != null)
        {
            // Instantiate the empty item (fuel slot placeholder)
            GameObject fuelObject = Instantiate(emptyItem);
            fuelObject.transform.SetParent(fuelSlot);
            fuelObject.transform.localScale = new Vector3(1f, 1f, 1f); 

            // Update the InventoryItem component of the instantiated object
            InventoryItem fuelItem = fuelObject.GetComponent<InventoryItem>();
            if (fuelItem == null)
            {
                fuelItem = fuelObject.AddComponent<InventoryItem>();
            }

            // Copy all information from the fuel object to the new InventoryItem
            fuelItem.item = fuel.item;
            fuelItem.count = fuel.count;
            fuelItem.gameObject.name = fuel.item.name;
            fuelItem.image = fuelItem.GetComponent<Image>();
            fuelItem.image.sprite = fuel.item.image;
            fuelItem.gameObject.GetComponent<Image>().sprite = fuel.item.image;
            fuelItem.RefreshCount();
        }
        else
        {
            // If there's no fuel, ensure the slot is cleared
            if (fuelSlot.childCount > 0)
            {
                Destroy(fuelSlot.GetChild(0).gameObject);
            }
        }

        // Input Slot (second child of the first child of internalUI)
        Transform inputSlot = internalUI.transform.GetChild(0).GetChild(1);

        // Check if input exists
        if (input != null)
        {
            // Instantiate the empty item (input slot placeholder)
            GameObject inputObject = Instantiate(emptyItem);
            inputObject.transform.SetParent(inputSlot);
            inputObject.transform.localScale = new Vector3(1f, 1f, 1f);

            // Update the InventoryItem component of the instantiated object
            InventoryItem inputItem = inputObject.GetComponent<InventoryItem>();
            if (inputItem == null)
            {
                inputItem = inputObject.AddComponent<InventoryItem>();
            }

            // Copy all information from the input object to the new InventoryItem
            inputItem.item = input.item;
            inputItem.count = input.count;
            inputItem.gameObject.name = input.item.name;
            inputItem.image = input.GetComponent<Image>();
            inputItem.image.sprite = input.item.image;
            inputItem.gameObject.GetComponent<Image>().sprite = input.item.image;
            inputItem.RefreshCount();
        }
        else
        {
            // If there's no input, ensure the slot is cleared
            if (inputSlot.childCount > 0)
            {
                Destroy(inputSlot.GetChild(0).gameObject);
            }
        }

        // Output Slot (third child of the first child of internalUI)
        Transform outputSlot = internalUI.transform.GetChild(0).GetChild(2);

        // Check if output exists
        if (output != null)
        {
            // Instantiate the empty item (output slot placeholder)
            GameObject outputObject = Instantiate(emptyItem);
            outputObject.transform.SetParent(outputSlot);
            outputObject.transform.localScale = new Vector3(1f, 1f, 1f);

            // Update the InventoryItem component of the instantiated object
            InventoryItem outputItem = outputObject.GetComponent<InventoryItem>();
            if (outputItem == null)
            {
                outputItem = outputObject.AddComponent<InventoryItem>();
            }

            // Copy all information from the output object to the new InventoryItem
            outputItem.item = output.item;
            outputItem.count = output.count;
            outputItem.gameObject.name = output.item.name;
            outputItem.image = outputItem.GetComponent<Image>();
            outputItem.image.sprite = output.item.image;
            outputItem.gameObject.GetComponent<Image>().sprite = output.item.image;
            outputItem.RefreshCount();
        }
        else
        {
            // If there's no output, ensure the slot is cleared
            if (outputSlot.childCount > 0)
            {
                Destroy(outputSlot.GetChild(0).gameObject);
            }
        }
    }

}