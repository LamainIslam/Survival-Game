using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public List<ResourceData> resources;
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
            // Create correct amount of items for each item type
            for (int i = 0; i < resources.Count; i++) {
                int amount = Random.Range(resources[i].min, resources[i].max - 1);
                for (int j = 0; j < amount; j++) {
                    Instantiate(resources[i].resource, transform.position, transform.rotation);
                }
            }
            Destroy(this.gameObject);
        }

        // Update slider values
        if (transform.childCount > 0) {
            if (transform.GetChild(0).name == "ResourceCanvas(Clone)") {
                Slider slider = gameObject.transform.GetChild(0).transform.GetChild(1).GetComponent<Slider>();
                slider.maxValue = maxHealth;
                slider.value = health;
            }
        }
    }
}


[System.Serializable]
public class ResourceData
{
    public GameObject resource;
    public int min;
    public int max;
}