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
    public DDOLManager Whatever;
    public static bool isInitialized = false;
    public static bool isFirstLoad = true;


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SceneManager.GetActiveScene().buildIndex == 9)
        {
            Debug.Log("in scene 9");
            Whatever = GameObject.Find("DontDestroyOnLoadObjectManager").GetComponent<DDOLManager>();

            if (Whatever != null)
            {
                if (!isInitialized)
                {
                    Whatever.enable();
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 9) // Skip handling for the loader scene
        {
            if (SceneNavigation.isFirstLoad && scene.buildIndex == 0) // Plains scene index
            {
                SceneNavigation.isFirstLoad = false; // Reset flag after first load
                return; // Skip teleportation
            }

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Player playerScript = player.GetComponent<Player>();

                GameObject teleportTarget = GameObject.Find("TeleportTarget");
                if (teleportTarget != null)
                {
                    //playerScript.Teleport(teleportTarget);
                }
                else
                {
                    Debug.LogWarning("TeleportTarget GameObject not found in the scene.");
                }
            }
            else
            {
                Debug.LogWarning("Player GameObject not found in the scene.");
            }
        }
    }

}
