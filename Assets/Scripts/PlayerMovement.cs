
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    // Dash settings
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 dashDirection = Vector3.zero;

    private float rotationX = 0;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    private CharacterController characterController;

    private bool canMove = true;
    private bool isDashing = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get movement directions
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Current speeds
        float currentWalkSpeed = walkSpeed;
        float currentRunSpeed = runSpeed;

        // Crouching
        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;

            currentWalkSpeed = crouchSpeed;
            currentRunSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
        }

        // Get movement input
        float curSpeedX = canMove
            ? (isRunning ? currentRunSpeed : currentWalkSpeed) * Input.GetAxis("Vertical")
            : 0;

        float curSpeedY = canMove
            ? (isRunning ? currentRunSpeed : currentWalkSpeed) * Input.GetAxis("Horizontal")
            : 0;

        // Store vertical movement
        float movementDirectionY = moveDirection.y;

        // Normal movement
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // --------------------------------
        // DASH
        // --------------------------------

        // Countdown dash cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Start dash
        if (Input.GetKeyDown(KeyCode.Q) &&
            canMove &&
            dashCooldownTimer <= 0 &&
            !isDashing)
        {
            // Get direction the player is currently moving
            dashDirection = moveDirection.normalized;

            // If the player isn't moving, dash forward
            if (dashDirection == Vector3.zero)
            {
                dashDirection = forward;
            }

            isDashing = true;

            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }

        // Apply dash movement
        if (isDashing)
        {
            moveDirection = dashDirection * dashSpeed;

            dashTimer -= Time.deltaTime;

            // End dash
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }

        // --------------------------------
        // JUMP
        // --------------------------------

        if (Input.GetButton("Jump") &&
            canMove &&
            characterController.isGrounded &&
            !isDashing)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // --------------------------------
        // GRAVITY
        // --------------------------------

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // --------------------------------
        // MOVE PLAYER
        // --------------------------------

        characterController.Move(moveDirection * Time.deltaTime);

        // --------------------------------
        // CAMERA / LOOK
        // --------------------------------

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;

            rotationX = Mathf.Clamp(
                rotationX,
                -lookXLimit,
                lookXLimit
            );

            playerCamera.transform.localRotation =
                Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(
                0,
                Input.GetAxis("Mouse X") * lookSpeed,
                0
            );
        }
    }
}


