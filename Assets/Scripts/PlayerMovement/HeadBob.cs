using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public Vector3 restPosition; // Local position where the object rests when not bobbing.
    public float transitionSpeed = 2f; // Smooth transition from moving to resting.
    public float bobSpeed = 4.8f; // Speed of the bobbing motion.
    public float bobAmount = 0.05f; // Amplitude of the bobbing motion.

    private float timer = Mathf.PI / 2; // Start at the crest of the sine wave.
    private bool dontHeadBob = false;
    void Update()
    {
        if (PlayerMovement.movementState == PlayerMovement.MovementState.Crouching || 
            PlayerMovement.movementState == PlayerMovement.MovementState.Sliding) {
            dontHeadBob = true;
        }
        // Check for movement input.
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && !dontHeadBob)
        {
            // Increment the timer based on the bobSpeed.
            timer += bobSpeed * Time.deltaTime;

            // Calculate the new position based on a sine wave for vertical motion and cosine wave for lateral motion.
            Vector3 newPosition = new Vector3(
                Mathf.Cos(timer) * bobAmount,
                restPosition.y + Mathf.Abs(Mathf.Sin(timer) * bobAmount),
                restPosition.z
            );

            // Apply the new position to the object.
            transform.localPosition = newPosition;
        }
        else
        {
            // Reset the timer when stopping and smoothly transition to the rest position.
            timer = Mathf.PI / 2;

            Vector3 newPosition = new Vector3(
                Mathf.Lerp(transform.localPosition.x, restPosition.x, transitionSpeed * Time.deltaTime),
                Mathf.Lerp(transform.localPosition.y, restPosition.y, transitionSpeed * Time.deltaTime),
                Mathf.Lerp(transform.localPosition.z, restPosition.z, transitionSpeed * Time.deltaTime)
            );

            // Apply the new position to the object.
            transform.localPosition = newPosition;
        }

        // Reset the timer to prevent it from growing indefinitely.
        if (timer > Mathf.PI * 2)
        {
            timer = 0;
        }
    }
}
