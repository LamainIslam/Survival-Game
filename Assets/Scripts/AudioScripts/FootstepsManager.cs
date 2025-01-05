using UnityEngine;
using UnityEngine.SceneManagement;

public class FootstepsManager : MonoBehaviour
{
    [Header("Plains Footsteps")]
    public AudioSource plainsWalk;
    public AudioSource plainsRun;

    [Header("Snow Footsteps")]
    public AudioSource snowWalk;
    public AudioSource snowRun;

    private AudioSource currentWalk;
    private AudioSource currentRun;

    // Singleton instance
    public static FootstepsManager Instance { get; private set; }
    private void Start()
    {
        // Set up footsteps for the initial scene
        SetupFootstepsForScene(SceneManager.GetActiveScene().buildIndex);

        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update footsteps when a new scene is loaded
        SetupFootstepsForScene(scene.buildIndex);
    }

    private void SetupFootstepsForScene(int sceneIndex)
    {
        // Disable all AudioSources initially
        DisableAllFootsteps();

        if (sceneIndex == 0) // Plains Scene
        {
            currentWalk = plainsWalk;
            currentRun = plainsRun;

            // Enable only plains footsteps
            EnableFootsteps(plainsWalk, plainsRun);
        }
        else if (sceneIndex == 8) // Snow Scene
        {
            currentWalk = snowWalk;
            currentRun = snowRun;

            // Enable only snow footsteps
            EnableFootsteps(snowWalk, snowRun);
        }
        else
        {
            // Handle other scenes or leave all footsteps disabled
            currentWalk = null;
            currentRun = null;
            Debug.Log("No footsteps available for this scene.");
            return;
        }

        // Ensure AudioSources are stopped before playing
        if (currentWalk != null) currentWalk.Stop();
        if (currentRun != null) currentRun.Stop();
    }

    private void DisableAllFootsteps()
    {
        if (plainsWalk != null) plainsWalk.enabled = false;
        if (plainsRun != null) plainsRun.enabled = false;
        if (snowWalk != null) snowWalk.enabled = false;
        if (snowRun != null) snowRun.enabled = false;
    }

    private void EnableFootsteps(AudioSource walkSource, AudioSource runSource)
    {
        if (walkSource != null) walkSource.enabled = true;
        if (runSource != null) runSource.enabled = true;
    }

    private void Update()
    {
        if (currentWalk == null || currentRun == null)
            return;

        // Check for movement input
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        if (isMoving)
        {
            if (isRunning)
            {
                // Sprinting: Play running sound
                if (!currentRun.isPlaying)
                {
                    currentWalk.Stop();
                    currentRun.Play();
                }
            }
            else
            {
                // Walking: Play walking sound
                if (!currentWalk.isPlaying)
                {
                    currentRun.Stop();
                    currentWalk.Play();
                }
            }
        }
        else
        {
            // Not moving: Stop all footsteps
            if (currentWalk.isPlaying) currentWalk.Stop();
            if (currentRun.isPlaying) currentRun.Stop();
        }
    }
}
