using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public int maxHunger = 100;
    public int currentHunger;

    public HealthBar healthBar;
    public HungerBar hungerBar;
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

    public void Heal(int amount)
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

    void IncreaseHunger(int amount)
    {
        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
        hungerBar.SetHunger(currentHunger);
        //Debug.Log("Hunger increased, current hunger: " + currentHunger);
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
