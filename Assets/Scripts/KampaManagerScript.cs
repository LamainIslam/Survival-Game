using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KampaManagerScript : MonoBehaviour
{
    public static KampaManagerScript Instance;

    private List<KampaAIScript> enemies = new List<KampaAIScript>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(KampaAIScript enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void AlertAllEnemies()
    {
        foreach (KampaAIScript enemy in enemies)
        {
            enemy.BecomeAggressive();
        }
    }
}