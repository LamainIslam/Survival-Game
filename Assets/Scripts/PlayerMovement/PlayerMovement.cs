using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Adjusments")]
    public float walkSpeed;
    public float runSpeed;
    public float drag;
    private float moveSpeed;
    private float desiredSpeed;
    private float lastDesiredSpeed;
    

    [Header("Jump Adjusments")]
    public float jumpForce;
    public float airMultiplier;
    public float jumpCooldown;

    [Header("Crouch Adjusments")]
    public float crouchSpeed;
    public float playerHeight;
    public float crouchScaleY;

    [Header("Slide Adjustments")]
    public float slideSpeed;
    public float maxSlideTime;
    public float slideForce;
    public float slideScaleY;
    private float slideTimer;
    public float slopeIncreaseMultiplier;
    public float speedIncreaseMultiplier;

    [Header("Layers")]
    public LayerMask ground;

    [Header("Booleans")]
    public bool isGrounded;
    public bool canJump;
    public bool jumped;
    public bool isSliding;
    private bool isLerping;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode slideKey = KeyCode.LeftAlt;

    [Header("Othes")]
    public Transform playerRotation;
    public TMP_Text speedText;
    
    public float maxSlopeAngle;

    
    private float horizontalInput;
    private float verticalInput;
    private float startScaleY;
    private Vector3 moveDirection;
    private RaycastHit slopeHit;
    
    Rigidbody rb;

    public MovementState movementState;
    public enum MovementState { 
        Walking,
        Sprinting,
        Crouching,
        Sliding,
        None
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canJump = true;

        startScaleY = transform.localScale.y;
    }

    private void FixedUpdate()
    {  
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        //raycast to find ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        
        if (isGrounded) { 
            rb.drag = drag; 
        } 
        else { 
            rb.drag = 0; 
        }

        MovementStateHandler();
        PlayerInput();
        SpeedCap();
    }

    private void PlayerInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //jumping
        if (Input.GetKey(jumpKey) && canJump && isGrounded) {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //sliding
        if (((rb.velocity.magnitude > walkSpeed) && Input.GetKeyDown(crouchKey)) || (Input.GetKeyDown(slideKey)))
        {
            StartSlide();
            isSliding = true;
        }
        if (isSliding)
        {
            SlidingMovement();
        }
        if (Input.GetKeyUp(crouchKey) || Input.GetKeyUp(slideKey) && isSliding)
        {
            EndSlide();
        }

        //Crouch
        if (!isSliding)
        {
            if (Input.GetKeyDown(crouchKey)) {
                transform.localScale = new Vector3(transform.localScale.x, crouchScaleY, transform.localScale.z);
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
            if (Input.GetKeyUp(crouchKey)){
                transform.localScale = new Vector3(transform.localScale.x, startScaleY, transform.localScale.z); 
            }
        }
       
    }

    private void MovementStateHandler()
    {
        if (isSliding)
        {
            movementState = MovementState.Sliding;
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredSpeed = slideSpeed;
            }
            else { 
                desiredSpeed = runSpeed; 
            }
        }
        else if (isGrounded && Input.GetKey(crouchKey) && !isSliding)
        {
            movementState = MovementState.Crouching;
            desiredSpeed = crouchSpeed;
        }
        else if (Input.GetKey(sprintKey))
        {
            movementState = MovementState.Sprinting;
            desiredSpeed = runSpeed;
        }
        else if (isGrounded)
        {
            movementState = MovementState.Walking;
            desiredSpeed = walkSpeed;
        }
        else
        {
            movementState = MovementState.None;
        }

        // Start Lerp if speed difference is significant and no Lerp is in progress
        if (Mathf.Abs(desiredSpeed - lastDesiredSpeed) > 1f && moveSpeed != 0 && !isLerping)
        {
            StartCoroutine(LerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredSpeed;  // Set speed directly if no Lerp is needed
        }

        lastDesiredSpeed = desiredSpeed;  // Store the last desired speed for next frame
    }

    private IEnumerator LerpMoveSpeed()
    {
        isLerping = true;
        float startVal = moveSpeed;
        float dif = Mathf.Abs(desiredSpeed - startVal);  // Difference in speed

        while (Mathf.Abs(moveSpeed - desiredSpeed) > 0.1f)  // Continue until close enough to desired speed
        {
            float speedLerpFactor = Time.deltaTime * speedIncreaseMultiplier;  // Speed factor
            moveSpeed = Mathf.Lerp(moveSpeed, desiredSpeed, speedLerpFactor);  // Lerp based on speed difference

            // Adjust speed for slopes
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeMultiplier = 1 + (slopeAngle / 90f);
                moveSpeed *= slopeMultiplier;  // Adjust for slope
            }

            yield return null;  // Wait for the next frame
        }

        moveSpeed = desiredSpeed;  // Ensure we end exactly at the desired speed
        isLerping = false;
    }


    private void Move()
    {
        moveDirection = playerRotation.forward * verticalInput + playerRotation.right * horizontalInput;

        //add force in the slope direction to make move feel better while going up slopes
        if (OnSlope() && !jumped) {
            rb.AddForce(GetSlopeDirection() * moveSpeed * 10f, ForceMode.Force);
        }

        if (isGrounded) { 
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force); 
        }
        //make it harder to move on air, air movement/air strafe can be turned off by making airMultiplier == 0
        else if (!isGrounded) { 
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force); 
        }

        //no gravity on slope results in more fluid movement on sloped surfaces
        rb.useGravity = !OnSlope();
    }

    private void SpeedCap() {
        if (OnSlope() && !jumped)
        {
            if (rb.velocity.magnitude > moveSpeed) { rb.velocity = rb.velocity.normalized * moveSpeed; }
        }
        else
        {
            //checks current velocity
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > (moveSpeed))
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump() {
        jumped = true;

        //resets y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //add y velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        canJump = true;
        jumped = false;
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void StartSlide() 
    {
        isSliding = true;
        slideTimer = maxSlideTime;

        transform.localScale = new Vector3(transform.localScale.x, slideScaleY, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
    private void EndSlide()
    {
        if (Input.GetKey(crouchKey)) { transform.localScale = new Vector3(transform.localScale.x, crouchScaleY, transform.localScale.z); }
        else{ transform.localScale = new Vector3(transform.localScale.x, startScaleY, transform.localScale.z); }
        isSliding = false;
    }
    private void SlidingMovement() 
    {
        if (!OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(moveDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else {
            rb.AddForce( GetSlopeDirection() * slideForce, ForceMode.Force);
            rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
        }
        
        if (slideTimer <= 0) { EndSlide(); }
    }

}
