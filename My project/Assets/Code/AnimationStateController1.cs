using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    [Header("References")]
    private Animator animator;
    private PlayerMovement playerMovement;
    private Rigidbody rb;

    [Header("Animation Parameters")]
    public float walkSpeedThreshold = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // Check if player is moving based on velocity
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        bool isMoving = horizontalVelocity.magnitude > walkSpeedThreshold;

        // Check movement state from PlayerMovement script
        if (playerMovement != null)
        {
            switch (playerMovement.state)
            {
                case PlayerMovement.MovementState.walking:
                    animator.SetBool("isWalking", isMoving);
                    animator.SetBool("isSprinting", false);
                    animator.SetBool("isCrouching", false);
                    break;

                case PlayerMovement.MovementState.sprinting:
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isSprinting", isMoving);
                    animator.SetBool("isCrouching", false);
                    break;

                case PlayerMovement.MovementState.crouching:
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isSprinting", false);
                    animator.SetBool("isCrouching", true);
                    break;

                case PlayerMovement.MovementState.air:
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isSprinting", false);
                    // Keep crouch state if was crouching
                    break;
            }

            // Set grounded parameter if you have landing/falling animations
            animator.SetBool("isGrounded", playerMovement.state != PlayerMovement.MovementState.air);
        }
        else
        {
            // Fallback to simple movement detection if PlayerMovement isn't available
            animator.SetBool("isWalking", isMoving);
        }
    }
}
