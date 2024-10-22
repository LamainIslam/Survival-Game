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
    public TMP_Text defenceStat;
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            IncreaseHunger(20);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        TakeDamage(20);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthBar.SetHealth(currentHealth);
        //Debug.Log("Player took damage, current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            //Die();
            //StopHealing();

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
        //Debug.Log("Player healed, current health: " + currentHealth);
    }

    IEnumerator DecreaseHungerOverTime()
    {
        while (true)
        {
            if (currentHunger > 0)
            {
                currentHunger--;
                hungerBar.SetHunger(currentHunger);
                //Debug.Log("Hunger decreased, current hunger: " + currentHunger);
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
        //Debug.Log("Hunger increased, current hunger: " + currentHunger);
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
        defenceStat.SetText($"Defence: {defence}");
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
