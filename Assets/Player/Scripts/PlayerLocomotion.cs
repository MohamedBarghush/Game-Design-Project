using UnityEngine;

namespace Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rb;
        
        PlayerManager playerManager;

        Vector3 moveDirection;
        Vector2 moveInput;
        Transform cameraObject;
        float moveAmount;

        [Header("Falling")]
        [HideInInspector] private float inAirTimer;
        [SerializeField] private float leapingVelocity = 0.5f;
        [SerializeField] private float fallingVelocity = 0.5f;
        [Tooltip("The height offset for the raycast to check if the player is grounded.")]
        [SerializeField] private float rayCastHeightOffset = 0.5f;
        [Tooltip("The distance of the raycast to check if the player is grounded.")]
        [SerializeField] private float rayCastDistance = 0.2f;
        [Tooltip("The radius of the sphere used to check if the player is grounded.")]
        [SerializeField] private float groundedSphereRadius = 0.15f;
        [Tooltip("The layer mask for which is supposed to be considered a ground to stand on.")]
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement Flags")]
        [SerializeField] private bool isSprinting = false;
        [SerializeField] public bool isGrounded = false;
        [SerializeField] public bool isCrouching = false;

        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float rotationSpeed = 15f;
        [Tooltip("The jump force applied to the player when jumping.")]
        [SerializeField] private float jumpVelocity = 5f;

        [SerializeField] private AudioSource stepsAS;

        public void LocomotionAwake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            playerManager = GetComponent<PlayerManager>();
            cameraObject = Camera.main.transform; // for calculating the movement direction based on the camera direction
        }

        // The general update function handling the player locomotion. (Being called in PlayerManager.cs)
        public void HandleMovement(InputHandler inputHandler, PlayerAnimatorManager animatorManager, bool isTargeting)
        {
            // Handle continous input
            moveInput = inputHandler.moveInput;
            isSprinting = inputHandler.sprintInput & !isTargeting;
            
            // Handle movement and rotation when on the ground
            moveAmount = Mathf.Clamp01(Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y));
            GroundLocomtion(); 
            if (!isTargeting || isCrouching) {
                HandleRotation();
            } else { 
                TargetingHandleRotation();
            }

            // Handling animations when targeting something and when not
            if (!isTargeting || isCrouching) animatorManager.UpdateAnimatorValues(0, moveAmount, isSprinting);
            else animatorManager.UpdateAnimatorValues(moveInput.x, moveInput.y, isSprinting);

            // Handling crouching logic
            if (isSprinting && isCrouching) ResetCrouching(animatorManager); // if tried running while crouching, cancel crouch
            HandleCrouching(inputHandler, animatorManager);

            // Handling rolling logic
            HandleRolling(inputHandler, animatorManager);
        }

        #region Movement and Rotation logic functions
        // Handle the movement and sprinting of the player based on the input and camera direction
        private void GroundLocomtion()
        {
            moveDirection = cameraObject.forward * moveInput.y +
                            cameraObject.right * moveInput.x;
            moveDirection.Normalize();
            moveDirection.y = 0f;

            if (isSprinting)
            {
                moveDirection *= sprintSpeed;
            }
            else
            {
                if (moveAmount >= 0.5f)
                {
                    moveDirection *= moveSpeed;
                }
                else
                {
                    moveDirection *= walkSpeed;
                }
            }

            Vector3 movementVelocity = moveDirection * moveSpeed;
            movementVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = movementVelocity;
        }

        // Handle the rotation of the player based on the camera direction and input
        private void HandleRotation()
        {
            Vector3 targetDirection = cameraObject.forward * moveInput.y +
                              cameraObject.right * moveInput.x;
            targetDirection.Normalize();
            targetDirection.y = 0f;

            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }

        // Specific rotation for when the player is targeting an enemy
        private void TargetingHandleRotation() {
            Vector3 cameraDirection = cameraObject.forward;
            cameraDirection.y = 0f; // Keep only horizontal direction
            cameraDirection.Normalize();

            if (cameraDirection == Vector3.zero)
                cameraDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }
        #endregion

        #region Jumping and falling logic functions
        public void HandleFallingAndLanding(bool isInteracting, bool isJumping, PlayerAnimatorManager animatorManager)
        {
            // Define the properties of the raycast that will check if the player is grounded
            RaycastHit hit;
            Vector3 rayCastOrigin = transform.position;
            Vector3 rayCastOriginForward = transform.position + transform.forward * 0.5f + Vector3.up * rayCastHeightOffset;
            Vector3 rayCastOriginBackward = transform.position - transform.forward * 0.5f + Vector3.up * rayCastHeightOffset;
            rayCastOrigin.y += rayCastHeightOffset;

            // Falling mid-air logic
            if (!isGrounded)
            {
                if (!isInteracting && !isJumping)
                {
                    animatorManager.PlayTargetAnimation("Falling", false);
                }

                rb.AddForce(transform.forward * leapingVelocity); // some force forward for realistic falling
                // Gravity force logic (increases the falling speed over time)
                inAirTimer += Time.deltaTime;
                rb.AddForce(Vector3.down * fallingVelocity * inAirTimer);
            }

            // Check if the player is grounded using a raycast and a sphere cast and trigger the landing animation
            if (Physics.Raycast(rayCastOrigin, Vector3.down, out hit, rayCastDistance, groundLayer))
            {
                if (!isGrounded && !isJumping)
                {
                    if (inAirTimer > 2.0f)
                    {
                        // Play the landing animation
                        animatorManager.anim.applyRootMotion = true;
                        animatorManager.PlayTargetAnimation("Land", true);
                    }
                    else
                    {
                        animatorManager.anim.applyRootMotion = false;
                        animatorManager.anim.SetTrigger("Land");
                    }
                }
            }
            if (Physics.OverlapSphere(rayCastOrigin, groundedSphereRadius, groundLayer).Length > 0 || 
                Physics.OverlapSphere(rayCastOriginForward, groundedSphereRadius, groundLayer).Length > 0 ||
                Physics.OverlapSphere(rayCastOriginBackward, groundedSphereRadius, groundLayer).Length > 0)
            {
                inAirTimer = 0;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        public void HandleJumping(bool isInteracting, bool isJumping, InputHandler inputHandler, PlayerAnimatorManager animatorManager)
        {
            if (inputHandler.jumpInput)
            {
                inputHandler.jumpInput = false;
                if (!isGrounded || isJumping || isInteracting) return;

                rb.AddForce(transform.up * jumpVelocity, ForceMode.Impulse);
                animatorManager.PlayTargetAnimation("Jump Running", false);
                animatorManager.anim.SetBool("isJumping", true);

                isCrouching = false; // Cancel crouch when jumping
            }
        }
        #endregion

        #region Crouching logic functions
        public void HandleCrouching(InputHandler inputHandler, PlayerAnimatorManager animatorManager)
        {
            if (inputHandler.crouchInput)
            {
                inputHandler.crouchInput = false;
                if (!isGrounded) return;
                isCrouching = !isCrouching;
                animatorManager.anim.applyRootMotion = false;
                if (isCrouching)
                {
                    inputHandler.sprintInput = false; // Cancel sprinting when crouching
                    // if (inputHandler.moveInput != Vector2.zero)
                    //     animatorManager.PlayTargetAnimation("Crouch State", false);
                    // else
                    //     animatorManager.PlayTargetAnimation("Crouch", false);
                    animatorManager.anim.SetBool("isCrouching", true);
                }
                else
                {
                    // animatorManager.anim.SetTrigger("Crouch");
                    animatorManager.anim.SetBool("isCrouching", false);
                }
            }
        }
        
        // Reset crouching state and animation for sprinting and jumping reset.
        public void ResetCrouching(PlayerAnimatorManager animatorManager)
        {
            isCrouching = false;
            animatorManager.anim.SetBool("isCrouching", false);
            animatorManager.anim.applyRootMotion = false; // Disable root motion when not crouching
        }
        #endregion

        public void HandleRolling(InputHandler inputHandler, PlayerAnimatorManager animatorManager)
        {
            if (inputHandler.rollInput)
            {
                inputHandler.rollInput = false;
                if (!isGrounded) return;

                Vector3 targetDirection = cameraObject.forward * moveInput.y +
                              cameraObject.right * moveInput.x;
                targetDirection.Normalize();
                targetDirection.y = 0f;
                
                transform.rotation = Quaternion.LookRotation(targetDirection);

                isCrouching = false; // Cancel crouch when rolling
                isSprinting = false; // Cancel sprinting when rolling
                animatorManager.anim.applyRootMotion = true;
                animatorManager.PlayTargetAnimation("Roll", true);
                playerManager.isInvulnerable = true; // Set invulnerability during roll
            }
        }

        public void VulnerableAgain() {
            playerManager.isInvulnerable = false;
            
        }

        public void WalkSound() {
            AudioManager.Instance?.PlaySoundAtSrc(SoundType.Walk, stepsAS, 0.2f);
        }

        public void RunSound() {
            AudioManager.Instance?.PlaySoundAtSrc(SoundType.Run, stepsAS, 0.2f);
        }   

        // ### Editor Gizmos ###
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * rayCastHeightOffset, groundedSphereRadius);
            Gizmos.DrawWireSphere(transform.position + transform.forward * 0.5f + Vector3.up * rayCastHeightOffset, groundedSphereRadius);
            Gizmos.DrawWireSphere(transform.position - transform.forward * 0.5f + Vector3.up * rayCastHeightOffset, groundedSphereRadius);
        }
    }
}
