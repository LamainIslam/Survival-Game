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
 

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 9)
        {
            Debug.Log("in scene 9");
            Whatever = GameObject.Find("DontDestroyOnLoadObjectManager").GetComponent<DDOLManager>();
            //GameObject[] SceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            if (Whatever != null)
            {
                if (!isInitialized)
                {
                    Whatever.enable();
                    isInitialized = true;
                }
                else
                {
                  //  Whatever.destroyer(); // Destroy duplicate object
                }
            }
            if (lastSelectedScene != null) {
                SceneManager.LoadScene(lastSelectedScene);
            }
            else {
                SceneManager.LoadScene(0);
            }
            
        }
        /*if (SceneManager.GetActiveScene().buildIndex == 7)
        {
            Whatever = GameObject.Find("DontDestroyOnLoadObjectManager").GetComponent<DDOLManager>(); ;
            if (Whatever != null) {
                Whatever.enable();
            }
        }*/
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
    public void replay() {
        if (lastSelectedScene == null) { 
            lastSelectedScene = dropdown.options[dropdown.value].text;
        }
        if (!isInitialized) { SceneManager.LoadScene(9); }
        else { SceneManager.LoadScene(lastSelectedScene); }
    }

    public void sceneSelector() {
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
}
