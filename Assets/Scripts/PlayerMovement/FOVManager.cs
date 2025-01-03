using UnityEngine;

public class FOVChanger : MonoBehaviour
{
    public Camera playerCamera; // Reference to the camera component
    public float defaultFOV = 60f; // The normal field of view
    public float maxFOV = 75f; // The field of view when moving
    public float transitionSpeed = 5f; // Speed of transition between FOV states

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }
    void Update()
    {
        float normalizedSpeed = Mathf.Clamp01(playerMovement.playerSpeed / playerMovement.runSpeed); // Normalize speed between 0 and 1
        float targetFOV = Mathf.Lerp(defaultFOV, maxFOV, normalizedSpeed); // Linearly interpolate between default and max FOV

        // Smoothly interpolate the FOV to the target value
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, transitionSpeed * Time.deltaTime);
    }
}
