using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneNavigation : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;
    private string selectedScene;
    public static string lastSelectedScene;
    public DDOLManager DDOLScript;
    public static bool isInitialized = false;
    public static bool isFirstLoad = true;


    private void Start()
    {
        //OnSceneLoaded(SceneManager.GetActiveScene());

        if (SceneManager.GetActiveScene().buildIndex == 9)
        {
            Debug.Log("in scene 9");
            DDOLScript = GameObject.Find("DontDestroyOnLoadObjectManager").GetComponent<DDOLManager>();

            if (DDOLScript != null)
            {
                Debug.Log("DDOL not null");
                if (!isInitialized)
                {
                    DDOLScript.enable();
                    Debug.Log("DDOL enabled");
                    isInitialized = true;
                }
            }

            if (lastSelectedScene != null)
            {
                SceneManager.LoadScene(lastSelectedScene);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        sceneSelector();
    }

    public void Play()
    {
        GameObject whatever = GameObject.Find("DontDestroyOnLoadObjectManager");
        if (whatever != null) {
            whatever.GetComponent<DDOLManager>().enable();
        }
        Debug.Log(isInitialized);
        selectedScene = dropdown.options[dropdown.value].text;
        lastSelectedScene = selectedScene;
        if (!isInitialized) { SceneManager.LoadScene(9); }
        else { SceneManager.LoadScene(lastSelectedScene); }
        
    }

    public void PlayOther()
    {
        selectedScene = dropdown.options[dropdown.value].text;
        lastSelectedScene = selectedScene;
        if (!isInitialized) { SceneManager.LoadScene(9); }
        else { SceneManager.LoadScene(lastSelectedScene); }
    }

    public void replay()
    {
        GameObject whatever = GameObject.Find("DontDestroyOnLoadObjectManager");
        if (whatever != null) {
            whatever.GetComponent<DDOLManager>().enable();
        }
        if (lastSelectedScene == null)
        {
            lastSelectedScene = dropdown.options[dropdown.value].text;
        }
        if (!isInitialized) { SceneManager.LoadScene(9); }
        else { SceneManager.LoadScene(lastSelectedScene); }
    }

    public void sceneSelector()
    {
        List<string> sceneNames = new List<string>();
        int sceneCount = EditorBuildSettings.scenes.Length;

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled) // Only include enabled scenes
            {
                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneNames.Add(sceneName);
            }
        }
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(sceneNames);
        }
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    private void OnSceneLoaded(Scene scene)
    {
        Debug.Log("Starting teleportation process...");

        if (scene.buildIndex == 9)
        {
            Debug.Log("Loader scene detected; skipping teleportation.");
            return; // Skip teleportation logic for the loader scene
        }

        GameObject player = FindPlayer();

        if (player == null)
        {
            Debug.LogWarning("Player not found on the first attempt. Retrying...");
            StartCoroutine(FindPlayerWithDelay(1.0f));
        }
        else
        {
            HandleTeleportation(player);
        }
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    private IEnumerator FindPlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject player = FindPlayer();

        if (player == null)
        {
            Debug.LogWarning("Player not found on the second attempt. Teleportation aborted.");
            yield break; // Exit if no player is found after retrying
        }

        HandleTeleportation(player);
    }

    private void HandleTeleportation(GameObject player)
    {
        Debug.Log("Player found; attempting to teleport.");
        Player playerScript = player.GetComponent<Player>();

        if (playerScript == null)
        {
            Debug.LogWarning("Player script not found on the Player GameObject.");
            return;
        }

        GameObject teleportTarget = GameObject.Find("TeleportTarget");

        if (teleportTarget != null)
        {
            playerScript.Teleport(teleportTarget);
            Debug.Log("Teleportation successful to target: " + teleportTarget.name);
        }
        else
        {
            Debug.LogWarning("TeleportTarget GameObject not found in the scene.");
        }
    }

}