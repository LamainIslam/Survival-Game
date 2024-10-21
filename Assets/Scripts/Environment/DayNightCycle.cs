using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light globalLight; // Reference to the directional light in the scene
    public Color dayColor = Color.white; // Color of the light during the day
    public Color nightColor = Color.blue; // Color of the light during the night
    public float dayDuration = 10f; // Duration of the day in seconds
    public float transitionDuration = 5f; // Duration of the transition in seconds
    public float nightDuration = 10f; // Duration of the night in seconds
    private bool isTransitioningToNight = false; // Indicates whether the cycle is transitioning to night

    void Start()
    {
        globalLight.color = dayColor;
        globalLight.intensity = 1f; // Full intensity during the day
        StartCoroutine(DayNightCycleRoutine());
    }

    void Update()
    {
        // No need to update currentTime here; it's managed in the coroutine
    }

    private System.Collections.IEnumerator DayNightCycleRoutine()
    {
        while (true)
        {
            // Day phase
            yield return new WaitForSeconds(dayDuration);

            // Transition to night
            yield return TransitionToNight();

            // Night phase
            yield return new WaitForSeconds(nightDuration);

            // Transition to day
            yield return TransitionToDay();
        }
    }

    private System.Collections.IEnumerator TransitionToNight()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate transition progress
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Interpolate color and intensity
            globalLight.color = Color.Lerp(dayColor, nightColor, t);
            globalLight.intensity = Mathf.Lerp(1f, 0.2f, t); // Decrease intensity

            yield return null; // Wait for the next frame
        }

        // Ensure it's fully transitioned to night
        globalLight.color = nightColor;
        globalLight.intensity = 0.2f;
        isTransitioningToNight = true;
    }

    private System.Collections.IEnumerator TransitionToDay()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate transition progress
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Interpolate color and intensity
            globalLight.color = Color.Lerp(nightColor, dayColor, t);
            globalLight.intensity = Mathf.Lerp(0.2f, 1f, t); // Increase intensity

            yield return null; // Wait for the next frame
        }

        // Ensure it's fully transitioned to day
        globalLight.color = dayColor;
        globalLight.intensity = 1f;
        isTransitioningToNight = false;
    }
}
