using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    /*
    // Initialise variables
    public Light globalLight;
    public Color dayColor;
    public Color nightColor;
    public float dayDuration = 10f;
    public float transitionDuration = 5f;
    public float nightDuration = 10f;

    void Start()
    {
        globalLight.color = dayColor;
        globalLight.intensity = 1f;
        StartCoroutine(DayNightCycleRoutine());
    }

    private System.Collections.IEnumerator DayNightCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dayDuration);
            yield return TransitionToNight();
            yield return new WaitForSeconds(nightDuration);
            yield return TransitionToDay();
        }
    }

    private System.Collections.IEnumerator TransitionToNight()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            globalLight.color = Color.Lerp(dayColor, nightColor, t);
            globalLight.intensity = Mathf.Lerp(1f, 0.2f, t);

            yield return null;
        }
        globalLight.color = nightColor;
        globalLight.intensity = 0.2f;
    }

    private System.Collections.IEnumerator TransitionToDay()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            globalLight.color = Color.Lerp(nightColor, dayColor, t);
            globalLight.intensity = Mathf.Lerp(0.2f, 1f, t);

            yield return null;
        }
        globalLight.color = dayColor;
        globalLight.intensity = 1f;
    }*/

    public float rotationSpeed;
    
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
