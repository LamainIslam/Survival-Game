using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    public TMPro.TMP_Dropdown difficultyDropdown; // Reference to the dropdown in the UI

    // Define stats for different difficulty levels
    [System.Serializable]
    public class DifficultySettings
    {
        public string difficultyName;
        public float playerDamageMultiplier = 1f;
        public float enemyDamageMultiplier = 1f;
        public float playerHealthMultiplier = 1f;
        public float enemyHealthMultiplier = 1f;
    }

    public DifficultySettings[] difficultySettings; // Array to store settings for each difficulty

    // Current multipliers
    public float playerDamageMultiplier { get; private set; } = 1f;
    public float enemyDamageMultiplier { get; private set; } = 1f;
    public float playerHealthMultiplier { get; private set; } = 1f;
    public float enemyHealthMultiplier { get; private set; } = 1f;

    private void Start()
    {
        if (difficultyDropdown != null)
        {
            int savedIndex = PlayerPrefs.GetInt("SelectedDifficulty", 0);
            difficultyDropdown.value = savedIndex;
            difficultyDropdown.onValueChanged.AddListener(SetDifficulty);
            SetDifficulty(savedIndex); // Initialize to saved difficulty
        }
        else
        {
            Debug.LogError("Difficulty dropdown is not assigned.");
        }

        //ApplyDifficultySettingsOnSceneLoad(); // Ensure settings are applied on scene load
    }

    public void SetDifficulty(int index)
    {
        if (index < 0 || index >= difficultySettings.Length)
        {
            Debug.LogError("Invalid difficulty index.");
            return;
        }

        DifficultySettings settings = difficultySettings[index];
        playerDamageMultiplier = settings.playerDamageMultiplier;
        enemyDamageMultiplier = settings.enemyDamageMultiplier;
        playerHealthMultiplier = settings.playerHealthMultiplier;
        enemyHealthMultiplier = settings.enemyHealthMultiplier;

        Debug.Log($"Difficulty set to: {settings.difficultyName}\n" +
                  $"Player Damage Multiplier: {playerDamageMultiplier}\n" +
                  $"Enemy Damage Multiplier: {enemyDamageMultiplier}\n" +
                  $"Player Health Multiplier: {playerHealthMultiplier}\n" +
                  $"Enemy Health Multiplier: {enemyHealthMultiplier}");

       // ApplyDifficultySettings();

        // Save the selected difficulty to PlayerPrefs
        PlayerPrefs.SetInt("SelectedDifficulty", index);
        PlayerPrefs.Save();
    }

    /*private void ApplyDifficultySettings()
    {
        // Example: Adjust stats globally in your game.
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.SetDamageMultiplier(playerDamageMultiplier);
            PlayerManager.Instance.SetHealthMultiplier(playerHealthMultiplier);
        }

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.SetDamageMultiplier(enemyDamageMultiplier);
            EnemyManager.Instance.SetHealthMultiplier(enemyHealthMultiplier);
        }

        Debug.Log("Difficulty settings applied successfully.");
    }*/

    /*private void ApplyDifficultySettingsOnSceneLoad()
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedDifficulty", 0);
        if (savedIndex >= 0 && savedIndex < difficultySettings.Length)
        {
            DifficultySettings settings = difficultySettings[savedIndex];
            playerDamageMultiplier = settings.playerDamageMultiplier;
            enemyDamageMultiplier = settings.enemyDamageMultiplier;
            playerHealthMultiplier = settings.playerHealthMultiplier;
            enemyHealthMultiplier = settings.enemyHealthMultiplier;

            ApplyDifficultySettings();
        }
        else
        {
            Debug.LogError("Invalid difficulty index in PlayerPrefs.");
        }
    }*/
}
