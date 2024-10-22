using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float rotationSpeed;
    
    void Update()
    {
        // Rotates light source
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
