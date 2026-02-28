using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    PhotonView photonView;
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
        photonView = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            PauseManager.Instance.RegisterLocalPlayer(this);
        }
    }
    void Update()
    {
        if (!photonView.IsMine)
            return;

        ApplyGravity();
        MovePlayer();
        RotateWithCamera();
        UpdateAnimations();

        if (Input.GetKey(KeyCode.Escape))
        {
            if (PauseManager.Instance.isPaused)
            {
                PauseManager.Instance.Resume();
            }
            else
            {
                PauseManager.Instance.Pause();
            }
        }
    }

    // ===== INPUT =====
    public void OnMove(InputValue value)
    {
        if (!photonView.IsMine)
            return;

        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!photonView.IsMine)
            return;
            
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