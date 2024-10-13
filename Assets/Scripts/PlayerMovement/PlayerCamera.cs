using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float xSens;
    public float ySens;
    public Transform playerRotation;
    private float xRotation;
    private float yRotation;
    public bool lockCursor;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        lockCursor = true;
    }

    void Update()
    {

        // Toggle for cursor being locked or unlocked
        // Assets/Scripts/PlayerMovement/PlayerCamera.cs
        if (lockCursor == true) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Mouse sensitivity
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens * Convert.ToInt32(lockCursor);
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens * Convert.ToInt32(lockCursor);

        // Camera and player rotation
        xRotation -= mouseY;
        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerRotation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}