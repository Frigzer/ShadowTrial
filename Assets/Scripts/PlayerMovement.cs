using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpHeight = 1.6f;
    public float gravity = -20f;
    // public float fallMultiplier = 2.5f;
    // public float lowJumpMultiplier = 2f;
    public float rotationSpeed = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float currentSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GroundCheck();
        Move();
        Jump();
        ApplyGravity();
        UpdateAnimator();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }

    private void Move()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * input.y + camRight * input.x;

        currentSpeed = move.magnitude;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        bool jumpHeld = jumpAction.action.IsPressed();

        // if (velocity.y < 0f)
        // {
        //     // szybsze opadanie
        //     velocity.y += gravity * fallMultiplier * Time.deltaTime;
        // }
        // if (velocity.y > 0f && !jumpHeld)
        // {
            // krótszy skok po puszczeniu przycisku
        //     velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        // }
        // else
        // {
            // normalne wznoszenie
            velocity.y += gravity * Time.deltaTime;
        // }

        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsGrounded", isGrounded);
    }
}