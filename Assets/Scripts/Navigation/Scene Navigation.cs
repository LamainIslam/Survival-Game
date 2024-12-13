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

    private void Start()
    {
        sceneSelector();
    }

    public void Play()
    {
        string selectedScene = dropdown.options[dropdown.value].text;
        SceneManager.LoadScene(selectedScene);
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

        dropdown.ClearOptions();
        dropdown.AddOptions(sceneNames);
    }

    public void Quit() 
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
