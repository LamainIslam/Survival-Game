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
    public float crouchSpeed;
    public float slideSpeed;
    public float drag;

    private float moveSpeed;
    private float desiredSpeed;
    private float lastDesiredSpeed;
    public float slopeIncreaseMultiplier;
    public float speedIncreaseMultiplier;

    [Header("Jump Adjusments")]
    public float jumpForce;
    public float airMultiplier;
    public float jumpCooldown;

    [Header("Crouch Adjusments")]
    public float playerHeight;
    public float crouchScaleY;

    [Header("Slide Adjustments")]
    public float maxSlideTime;
    public float slideForce;
    public float slideScaleY;
    private float slideTimer;

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
        
        if (isGrounded) { rb.drag = drag; } else { rb.drag = 0; }

        MovementStateHandler();
        PlayerInput();
        SpeedCap();

        speedText.text = ("Speed: ") + (rb.velocity.magnitude * 1.0f).ToString("0");
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
        if ((rb.velocity.magnitude > walkSpeed) && Input.GetKeyDown(crouchKey))
        {
            StartSlide();
            isSliding = true;
        }
        if (isSliding)
        {
            SlidingMovement();
        }
        if (Input.GetKeyUp(crouchKey) && isSliding)
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

    private void MovementStateHandler() {
        if (isSliding) {
            movementState = MovementState.Sliding;
            if (OnSlope() && rb.velocity.y < 0.1f) {
                desiredSpeed = slideSpeed;
            }
            else { desiredSpeed = runSpeed; }
        }

        else if ((isGrounded && Input.GetKey(crouchKey)) && !isSliding)
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
        else { movementState = MovementState.None; }

        if (Mathf.Abs(desiredSpeed - lastDesiredSpeed) > 6f && moveSpeed != 0)
        {
            StartCoroutine(LerpMoveSpeed());
        }
        else {
            moveSpeed = desiredSpeed;
        }
        lastDesiredSpeed = desiredSpeed;
    }

    private IEnumerator LerpMoveSpeed()
    {
        isLerping = true;
        float time = 0f;
        float duration = 3f;
        float startVal = moveSpeed;

        while (time < duration)
        {
            moveSpeed = Mathf.Lerp(startVal, desiredSpeed, time / duration);
            time += Time.deltaTime;

            // Adjust speed for slopes
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeMultiplier = 1 + (slopeAngle / 90f);
                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeMultiplier;
            }
            yield return null;
        }

        moveSpeed = desiredSpeed;
        isLerping = false;
    }

    private void Move()
    {
        moveDirection = playerRotation.forward * verticalInput + playerRotation.right * horizontalInput;

        if (OnSlope() && !jumped) {
            rb.AddForce(GetSlopeDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0) { rb.AddForce(Vector3.down * 120f, ForceMode.Force); }
        }

        if (isGrounded) { rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force); }
        else if (!isGrounded) { rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force); }

        rb.useGravity = !OnSlope();
    }

    private void SpeedCap() {
        if (OnSlope() && !jumped)
        {
            if (rb.velocity.magnitude > moveSpeed) { rb.velocity = rb.velocity.normalized * moveSpeed; }
        }
        else
        {

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
        transform.localScale = new Vector3(transform.localScale.x, startScaleY, transform.localScale.z);
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
            rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
        }
        
        if (slideTimer <= 0) { EndSlide(); }
    }

}
