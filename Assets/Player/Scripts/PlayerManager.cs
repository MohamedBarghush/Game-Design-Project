using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(InputHandler))]
    [RequireComponent(typeof(PlayerLocomotion))]
    [RequireComponent(typeof(PlayerAnimatorManager))]
    public class PlayerManager : MonoBehaviour
    {
        [HideInInspector] public InputHandler inputHandler;
        [HideInInspector] public PlayerLocomotion locomotion;
        [HideInInspector] public PlayerAnimatorManager animatorManager;
        [HideInInspector] public PlayerAuraManager auraManager;
        [HideInInspector] public PlayerShield playerShield;
        [HideInInspector] public PlayerAttack playerAttack;
        [HideInInspector] public PlayerHealth playerHealth;
        [HideInInspector] public PlayerTrolling playerTrolling;

        [SerializeField] private bool isInteracting;
        [SerializeField] private bool isJumping;
        [SerializeField] public bool isBlocking;
        [SerializeField] public bool isTargeting;
        [SerializeField] public bool isDead;
        [SerializeField] public bool isInvulnerable;
        
        // [HideInInspector] public CameraManager cameraManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            locomotion = GetComponent<PlayerLocomotion>();
            animatorManager = GetComponent<PlayerAnimatorManager>();
            auraManager = GetComponent<PlayerAuraManager>();
            playerHealth = GetComponent<PlayerHealth>();
            playerTrolling = GetComponent<PlayerTrolling>();


            TryGetComponent(out playerShield);
            auraManager?.PlayerAuraStart(animatorManager, inputHandler);
            TryGetComponent(out playerAttack);

            // cameraManager = FindFirstObjectByType<CameraManager>();z
            playerHealth.HealthAwake(animatorManager);
            locomotion.LocomotionAwake();
        }

        // Update is called once per frame
        private void Update()
        {
            if (isDead) return;
            playerHealth.HealthHandler();
            playerShield?.HandleShield(isJumping, locomotion.isGrounded, isInteracting, inputHandler, animatorManager);
            playerAttack?.HandleAttacking(locomotion, inputHandler, animatorManager);
            playerTrolling?.HandleTrolling(inputHandler, animatorManager);
        }

        private void FixedUpdate()
        {
            auraManager?.HandleAura(isInteracting);
            if (isInteracting) return;
            locomotion.HandleMovement(inputHandler, animatorManager, isTargeting);
        }

        private void LateUpdate()
        {
            // cameraManager.HandleCameraMovement(inputHandler);
            isInteracting = animatorManager.anim.GetBool("isInteracting");
            isJumping = animatorManager.anim.GetBool("isJumping");
            // animatorManager.anim.SetBool("Targeting", isTargeting);
            // if (isInteracting) return;
            locomotion.HandleFallingAndLanding(isInteracting, isJumping, animatorManager);
            locomotion.HandleJumping(isInteracting, isJumping, inputHandler, animatorManager);
        }
    }
}
