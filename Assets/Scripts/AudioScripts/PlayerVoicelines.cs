using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoiceLineTrigger : MonoBehaviour
{
    public AudioSource voiceLine1; // Assign the AudioSource for VoiceLineCollider
    public AudioSource voiceLine2; // Assign the AudioSource for VoiceLineCollider2
    public InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check for the first trigger
        if (other.CompareTag("HeightVoiceLineTrigger"))
        {
            PlayVoiceLine(voiceLine1);
        }
        // Check for the second trigger
        else if (other.CompareTag("NearBoss"))
        {
            for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
            {
                if (inventoryManager.inventorySlots[i].gameObject.transform.childCount > 0)
                {
                    GameObject GO = inventoryManager.inventorySlots[i].gameObject.transform.GetChild(0).gameObject;
                    if (GO.name == "Dark Sword")
                    {
                        PlayVoiceLine(voiceLine2);
                    }
                }
            }
        }
    }

    private void PlayVoiceLine(AudioSource audioSource)
    {
        // Play the voice line if the audio source is valid and not already playing
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}


