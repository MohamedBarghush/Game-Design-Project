using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rb;

        Vector3 moveDirection;
        Vector2 moveInput;
        // float moveAmount;
        Transform cameraObject;

        float moveAmount;

        [Header("Falling")]
        [SerializeField] private float inAirTimer;
        [SerializeField] private float leapingVelocity = 0.5f;
        [SerializeField] private float fallingVelocity = 0.5f;
        [SerializeField] private float rayCastHeightOffset = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement Flags")]
        [SerializeField] private bool isSprinting = false;
        [SerializeField] public bool isGrounded = false;
        [SerializeField] private bool isCrouching = false;

        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float jumpVelocity = 5f;

        void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            cameraObject = Camera.main.transform;
        }

        public void HandleMovement(InputHandler inputHandler, PlayerAnimatorManager animatorManager, bool isTargeting)
        {
            moveInput = inputHandler.moveInput;
            isSprinting = inputHandler.sprintInput & !isTargeting;
            moveAmount = Mathf.Clamp01(Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y));
            GroundLocomtion(); 
            if (!isTargeting || isCrouching) {
                HandleRotation();
            } else { 
                TargetingHandleRotation();
            }
            if (isSprinting && isCrouching) ResetCrouching(animatorManager); // if tried running while crouching, cancel crouch
            HandleCrouching(inputHandler, animatorManager);
            if (!isTargeting || isCrouching) animatorManager.UpdateAnimatorValues(0, moveAmount, isSprinting);
            else animatorManager.UpdateAnimatorValues(moveInput.x, moveInput.y, isSprinting);
            HandleRolling(inputHandler, animatorManager);
        }

        // Update is called once per frame
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

        public void HandleFallingAndLanding(bool isInteracting, bool isJumping, PlayerAnimatorManager animatorManager)
        {
            RaycastHit hit;
            Vector3 rayCastOrigin = transform.position;
            rayCastOrigin.y += rayCastHeightOffset;

            // isGrounded = true;

            if (!isGrounded)
            {
                if (!isInteracting && !isJumping)
                {
                    animatorManager.PlayTargetAnimation("Falling", false);
                }

                inAirTimer += Time.deltaTime;
                rb.AddForce(transform.forward * leapingVelocity);
                rb.AddForce(Vector3.down * fallingVelocity * inAirTimer);
            }

            if (Physics.Raycast(rayCastOrigin, Vector3.down, out hit, 0.2f, groundLayer))
            {
                if (!isGrounded && !isJumping)
                {
                    animatorManager.anim.applyRootMotion = true;
                    // isInteracting = true;
                    animatorManager.PlayTargetAnimation("Land", true);
                }
            }

            if (Physics.OverlapSphere(rayCastOrigin, 0.15f, groundLayer).Length > 0)
            {
                // if (!isGrounded && isInteracting) {
                //     animatorManager.PlayTargetAnimation("Land", true);
                //     animatorManager.anim.applyRootMotion = true;
                // }

                inAirTimer = 0;
                isGrounded = true;
                // isInteracting = false;
                // Debug.Log("Grounded: " + hit.transform.name);
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
                // animatorManager.anim.applyRootMotion = true;

                isCrouching = false; // Cancel crouch when jumping
            }
        }

        public void HandleCrouching(InputHandler inputHandler, PlayerAnimatorManager animatorManager)
        {
            if (inputHandler.crouchInput)
            {
                inputHandler.crouchInput = false;
                if (!isGrounded) return;
                isCrouching = !isCrouching;
                if (isCrouching)
                {
                    inputHandler.sprintInput = false; // Cancel sprinting when crouching
                    if (inputHandler.moveInput != Vector2.zero)
                        animatorManager.PlayTargetAnimation("Crouch State", false);
                    else
                        animatorManager.PlayTargetAnimation("Crouch", false);
                }
                else
                {
                    animatorManager.anim.SetTrigger("Crouch");
                }
            }
        }

        public void ResetCrouching(PlayerAnimatorManager animatorManager)
        {
            isCrouching = false;
            animatorManager.anim.SetTrigger("Crouch");
        }

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
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * rayCastHeightOffset, 0.2f);
        }
    }
}
