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
    public float rotationSpeed = 12f;
    public float fallMultiplier = 1.7f;
    public float lowJumpMultiplier = 1.8f;

    [Header("Ground Check")]
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundMask;

    [Header("Fall")]
    public float fallTimeout = 0.15f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Animation")]
    public Animator animator;

    [Header("Audio")]
    public AudioClip landingAudioClip;
    public AudioClip[] footstepAudioClips;
    [Range(0f, 1f)] public float footstepVolume = 0.5f;
    [Range(0f, 1f)] public float landingVolume = 0.5f;

    [Header("Landing")]
    public float minFallTimeForLanding = 0.12f;
    public float minFallVelocityForLanding = -6f;

    private CharacterController controller;
    private Vector3 velocity;
    private MovingPlatform currentPlatform;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float currentSpeed;

    private float fallTimeoutDelta;
    private bool wasGroundedLastFrame;
    private float airTime;
    private bool wasFalling;
    private float verticalVelocityBeforeGroundCheck;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int ShouldFallHash = Animator.StringToHash("ShouldFall");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        fallTimeoutDelta = fallTimeout;
        wasGroundedLastFrame = isGrounded;
    }

    private void Update()
    {
        verticalVelocityBeforeGroundCheck = velocity.y;
        
        GroundCheck();
        CheckMovingPlatform();
        ApplyPlatformMovement();
        Move();
        Jump();
        ApplyGravity();
        UpdateAnimator();
        HandleLandingAudio();

        if (isGrounded)
            airTime = 0f;
        else
            airTime += Time.deltaTime;

        wasGroundedLastFrame = isGrounded;
    }

    private void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y + groundedOffset,
            transform.position.z
        );

        isGrounded = Physics.CheckSphere(
            spherePosition,
            groundedRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (isGrounded)
        {
            fallTimeoutDelta = fallTimeout;

            if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            if (fallTimeoutDelta > 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
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

            if (animator != null)
            {
                animator.SetTrigger(JumpHash);
            }
        }
    }

    private void ApplyGravity()
    {
        bool jumpHeld = jumpAction.action.IsPressed();

        if (velocity.y < 0f)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else if (velocity.y > 0f && !jumpHeld)
        {
            velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat(SpeedHash, currentSpeed);
        animator.SetBool(IsGroundedHash, isGrounded);

        bool shouldFall = !isGrounded && velocity.y < -0.1f && fallTimeoutDelta <= 0f;
        animator.SetBool(ShouldFallHash, shouldFall);
    }

    private void HandleLandingAudio()
    {
        bool wasAirborneLongEnough = airTime >= minFallTimeForLanding;
        bool wasFallingFastEnough = velocity.y <= minFallVelocityForLanding;

        if (!wasGroundedLastFrame && isGrounded && landingAudioClip != null &&
            (wasAirborneLongEnough || wasFallingFastEnough))
        {
            AudioSource.PlayClipAtPoint(
                landingAudioClip,
                transform.TransformPoint(controller.center),
                landingVolume
            );
        }
    }

    public void OnFootstep()
    {
        if (!isGrounded) return;
        if (footstepAudioClips == null || footstepAudioClips.Length == 0) return;

        int index = Random.Range(0, footstepAudioClips.Length);
        AudioSource.PlayClipAtPoint(
            footstepAudioClips[index],
            transform.TransformPoint(controller.center),
            footstepVolume
        );
    }

    private void CheckMovingPlatform()
    {
        if (!isGrounded)
        {
            currentPlatform = null;
            return;
        }

        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y + groundedOffset,
            transform.position.z
        );

        Collider[] hits = Physics.OverlapSphere(
            spherePosition,
            groundedRadius + 0.05f,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        MovingPlatform detectedPlatform = null;

        foreach (Collider hit in hits)
        {
            MovingPlatform platform = hit.GetComponentInParent<MovingPlatform>();
            if (platform != null)
            {
                detectedPlatform = platform;
                break;
            }
        }

        currentPlatform = detectedPlatform;
    }

    private void ApplyPlatformMovement()
    {
        if (currentPlatform == null) return;

        if (currentPlatform.DeltaMovement != Vector3.zero)
        {
            controller.Move(currentPlatform.DeltaMovement);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        MovingPlatform platform = hit.collider.GetComponentInParent<MovingPlatform>();

        if (platform != null)
        {
            if (hit.normal.y > 0.3f)
            {
                currentPlatform = platform;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded
            ? new Color(0f, 1f, 0f, 0.35f)
            : new Color(1f, 0f, 0f, 0.35f);

        Vector3 spherePosition = new Vector3(
            transform.position.x,
            transform.position.y + groundedOffset,
            transform.position.z
        );

        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }
}