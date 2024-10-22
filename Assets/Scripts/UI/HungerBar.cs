using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public Slider slider;

    // Sets max hunger
    public void SetMaxHunger(float hunger)
    {
        slider.maxValue = hunger;
        slider.value = hunger;
    }

    // Sets hunger
    public void SetHunger(float hunger)
    {
        slider.value = hunger;
    }
}
