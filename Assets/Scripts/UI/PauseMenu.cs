using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // Reference to the Pause Menu GameObject
    public bool isPaused = true;
    public GameObject crossHair;

    // Start is called before the first frame update
    void Start()
    {
        if (pauseMenu != null) {
            pauseMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        GameObject.Find("PlayerCameraHolder").transform.GetChild(0).transform.GetChild(0).GetComponent<PlayerCamera>().lockCursor = false;
        crossHair.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;
        GameObject.Find("PlayerCameraHolder").transform.GetChild(0).transform.GetChild(0).GetComponent<PlayerCamera>().lockCursor = true;
        crossHair.SetActive(true);
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();   
        }
        else
        {
            PauseGame();
        }
    }

    public void Quit() {
        Application.Quit();
    }

    public void Menu() {
        SceneManager.LoadScene("MainMenu");
    }
}
