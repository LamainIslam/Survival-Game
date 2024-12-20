using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentHunger = maxHunger;
        hungerBar.SetMaxHunger(maxHunger);
        healingCoroutine = StartCoroutine(HealOverTime());
        hungerCoroutine = StartCoroutine(DecreaseHungerOverTime());
    }

    void Update()
    {
    
    }
    
    private void OnTriggerEnter(Collider other)
    {
        TakeDamage(20);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damaged");
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

    // Updates current defence
    public void UpdateDefence()
    {
        InventoryManager inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        defence = 0;
        for (int i = 0; i < inventoryManager.armourSlots.Length; i++) {
            InventoryItem itemInSlot = inventoryManager.armourSlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null) {
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
}
