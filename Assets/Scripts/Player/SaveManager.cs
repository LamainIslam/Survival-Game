using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public void SaveGame(Player player)
    {
        SaveData data = new SaveData
        {
            playerHealth = player.currentHealth,
            playerHunger = player.currentHunger,
            playerDefence = player.defence,
            playerPosition = player.transform.position,
            currentScene = SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game saved at: {saveFilePath}");
    }

    public void LoadGame(Player player)
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Apply loaded data to player
            player.currentHealth = data.playerHealth;
            player.currentHunger = data.playerHunger;
            player.defence = data.playerDefence;
            player.transform.position = data.playerPosition;

            // Load the saved scene
            SceneManager.LoadScene(data.currentScene);

            Debug.Log("Game loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No save file found.");
        }
    }
}
