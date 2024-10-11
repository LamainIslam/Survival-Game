using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Adjusments")]
    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;
    public float jumpForce;
    public float drag;
    public float airMultiplier;
    public float jumpCooldown;
    public float playerHeight;
    public float crouchScaleY;
    

    [Header("Layers")]
    public LayerMask ground;

    [Header("Booleans")]
    public bool isGrounded;
    public bool canJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Othes")]
    public Transform playerRotation;
    
    public float moveSpeed;
    private float horizontalInput;
    private float verticalInput;
    private float startScaleY;
    private Vector3 moveDirection;
    
    Rigidbody rb;

    public MovementState movementState;
    public enum MovementState { 
        Walking,
        Sprinting,
        Crouching,
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
    }

    private void PlayerInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump && isGrounded) {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Crouch
        if (Input.GetKeyDown(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchScaleY, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey)){
            transform.localScale = new Vector3(transform.localScale.x, startScaleY, transform.localScale.z); 
        }
    }

    private void MovementStateHandler() {
        if (isGrounded && Input.GetKey(crouchKey)) {
            movementState = MovementState.Crouching;
            moveSpeed = crouchSpeed;
            Debug.Log("test");
        }
        
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            movementState = MovementState.Sprinting;
            moveSpeed = runSpeed;
            Debug.Log("test1");
        }

        else
        {
            movementState = MovementState.Walking;
            moveSpeed = walkSpeed;
        }
    }

    private void Move()
    {
        moveDirection = playerRotation.forward * verticalInput + playerRotation.right * horizontalInput;
        if (isGrounded) { rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force); }
        else if (!isGrounded) { rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force); }
    }

    private void SpeedCap() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump() {
        //resets y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //add y velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        canJump = true;
    }
}
