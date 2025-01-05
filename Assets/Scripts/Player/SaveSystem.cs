using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float health;
    public float hunger;
    public Vector3 position;
    public string currentScene;
}

public class SaveSystem : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "playerSave.json");

    public static void SaveGame(Player player)
    {
        PlayerData data = new PlayerData
        {
            health = player.currentHealth,
            hunger = player.currentHunger,
            position = player.transform.position,
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Game saved to {SavePath}");
    }

    public static void LoadGame(Player player)
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            player.currentHealth = data.health;
            player.currentHunger = data.hunger;
            player.transform.position = data.position;
            UnityEngine.SceneManagement.SceneManager.LoadScene(data.currentScene);

            Debug.Log("Game loaded successfully.");
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }
}
