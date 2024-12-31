using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public GameObject[] resources;
    public int tier;

    void Start ()
    {
        // Start health to be maxHealth
        health = maxHealth;
    }

    void Update()
    {
        // Destroy gameobject and create resource when no health 
        if (health <= 0) {
            for (int i = 0; i < resources.Length; i++) {
                Instantiate(resources[i], transform.position, transform.rotation);
            }
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
