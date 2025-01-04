using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public TMP_Text damageDoneText;
    public void Intialize(float damageDone, Color color) {
        Debug.Log($"Initialized with damage: {damageDone}, color: {color}");
        Debug.Log("instialized!");
        Destroy(this.gameObject, 2.01f);

        damageDoneText.text = "";
        damageDoneText.text = "-";
        damageDone = Mathf.Round(damageDone);
        damageDoneText.text += damageDone.ToString();
        damageDoneText.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
        //transform.position += Vector3.up * 0.01f;
    }
}
