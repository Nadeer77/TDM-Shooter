using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float jumpForce = 4f;
    public float rotationSpeed = 12f;

    private Vector2 moveInput;

    [Header("Gravity")]
    private float gravity = -9.81f;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        ApplyGravity();
        MovePlayer();
        RotateWithCamera();
        UpdateAnimations();
    }

    // ===== INPUT =====
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    // ===== MOVEMENT =====
    void MovePlayer()
    {
        // Camera-relative movement
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 move =
            camRight.normalized * moveInput.x +
            camForward.normalized * moveInput.y;

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    // ===== ROTATION (KEY FIX) =====
    void RotateWithCamera()
    {
        // Rotate ONLY when moving forward/back
        if (moveInput.y > 0.1f)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // ===== GRAVITY =====
    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    // ===== ANIMATIONS =====
    void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("Forward", moveInput.y);
        animator.SetFloat("Sideways", moveInput.x);
    }
}