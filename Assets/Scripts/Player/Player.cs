using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    public float maxHunger = 100;
    public float currentHunger;

    public float defence;

    public HealthBar healthBar;
    public HungerBar hungerBar;
    public DefenceUI defenceUI;
    public float respawnDelay = 1f;
    public float healInterval = 1f;
    public float hungerInterval = 1f;
    private Coroutine healingCoroutine;
    private Coroutine hungerCoroutine;
    GameObject teleportTarget;
    private bool hasRun = false;

    void Awake()
    {
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public SaveManager saveManager;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentHunger = maxHunger;
        hungerBar.SetMaxHunger(maxHunger);

        saveManager = FindObjectOfType<SaveManager>();

        healingCoroutine = StartCoroutine(HealOverTime());
        hungerCoroutine = StartCoroutine(DecreaseHungerOverTime());
    }

    void Update()
    {
        if (!hasRun) {
            Debug.Log("PING!");
            teleportTarget = GameObject.Find("TeleportTarget");
            if (teleportTarget != null) {    
                Teleport(teleportTarget);

                Debug.Log("Teleportation successful to target: " + teleportTarget.name);
                hasRun = true;
            }
            else
            {
                Debug.LogWarning("TeleportTarget GameObject not found in the scene.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damaged");
        float effectiveDefence = defenceUI.defence > 0 ? defenceUI.defence : 1; // Prevent division by 0
        currentHealth -= damage * damage / (damage + defenceUI.defence);
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            PlayerDeath.shouldDie = true;
        }
    }

    IEnumerator HealOverTime()
    {
        while (true)
        {
            if (currentHealth < maxHealth)
            {
                Heal(1);
            }
            yield return new WaitForSeconds(healInterval);
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator DecreaseHungerOverTime()
    {
        while (true)
        {
            if (currentHunger > 0)
            {
                currentHunger--;
                hungerBar.SetHunger(currentHunger);
            }
            yield return new WaitForSeconds(hungerInterval);
        }
    }

    public void IncreaseHunger(float amount)
    {
        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
        hungerBar.SetHunger(currentHunger);
    }

    public void UpdateDefence()
    {
        InventoryManager inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        defence = 0;
        for (int i = 0; i < inventoryManager.armourSlots.Length; i++)
        {
            InventoryItem itemInSlot = inventoryManager.armourSlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                defence += itemInSlot.item.defencePoints;
            }
        }
        defenceUI.UpdateDefence(defence);
    }

    void Die()
    {
        Invoke("Respawn", respawnDelay);
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        healthBar.SetMaxHealth(maxHealth);
        hungerBar.SetMaxHunger(maxHunger);
        transform.position = Vector3.zero;
    }

    void StopHealing()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }
        if (hungerCoroutine != null)
        {
            StopCoroutine(hungerCoroutine);
            hungerCoroutine = null;
        }
    }

    public void Teleport(GameObject teleportTo)
    {
        if (teleportTo != null)
        {
            transform.position = teleportTo.transform.position;
            Debug.Log($"Teleported to: {teleportTo.name}");
        }
        else
        {
            Debug.LogWarning("Teleport failed: Target GameObject is null.");
        }
    }

    // Save game data
    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        SaveSystem.LoadGame(this);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the flag when a new scene is loaded
        hasRun = false;
    }
}
