using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DefenceUI : MonoBehaviour
{
    public float defence;
    public TMP_Text defenceText;

    public void UpdateDefence(float newDefence)
    {
        defence = newDefence;
        defenceText.SetText($"Defence: {defence}");
    }
}
