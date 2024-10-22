using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDeath : MonoBehaviour
{
    public MonoBehaviour enemyScript;
    public GameObject enemyDrop;
    public float health;
    public float maxHealth;

    void Start ()
    {
        // Set maxHealth
        if (enemyScript != null)
        {
            if (enemyScript is AIScriptHostile)
            {
                maxHealth = (enemyScript as AIScriptHostile).maxHealth;
            }
            else if (enemyScript is AIScriptNeutral)
            {
                maxHealth = (enemyScript as AIScriptNeutral).maxHealth;
            }
            else if (enemyScript is AIScriptPassive)
            {
                maxHealth = (enemyScript as AIScriptPassive).maxHealth;
            }
        }
    }

    void Update()
    {
        // Update health
        if (enemyScript != null)
        {
            if (enemyScript is AIScriptHostile)
            {
                health = (enemyScript as AIScriptHostile).health;
            }
            else if (enemyScript is AIScriptNeutral)
            {
                health = (enemyScript as AIScriptNeutral).health;
            }
            else if (enemyScript is AIScriptPassive)
            {
                health = (enemyScript as AIScriptPassive).health;
            }
        }

        // Destroy gameobject and create enemy drop when no health 
        if (health <= 0) {
            Instantiate(enemyDrop, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }

        // Update slider values
        if (transform.childCount > 0) {
            Slider slider = gameObject.transform.GetChild(0).transform.GetChild(1).GetComponent<Slider>();
            slider.maxValue = maxHealth;
            slider.value = health;
        }
    }
}
