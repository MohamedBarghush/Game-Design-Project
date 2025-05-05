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
        // [HideInInspector] public CameraManager cameraManager;

        [SerializeField] private bool isInteracting;
        [SerializeField] private bool isJumping;
        [SerializeField] public bool isBlocking;
        [SerializeField] public bool isTargeting;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            locomotion = GetComponent<PlayerLocomotion>();
            animatorManager = GetComponent<PlayerAnimatorManager>();
            auraManager = GetComponent<PlayerAuraManager>();


            TryGetComponent(out playerShield);
            auraManager?.PlayerAuraStart(animatorManager, inputHandler);
            // cameraManager = FindFirstObjectByType<CameraManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            playerShield?.HandleShield(isJumping, locomotion.isGrounded, isInteracting, inputHandler, animatorManager);
        }

        private void FixedUpdate()
        {
            auraManager.HandleAura(isInteracting);
            if (isInteracting) return;
            locomotion.HandleMovement(inputHandler, animatorManager, isTargeting);
        }

        private void LateUpdate()
        {
            // cameraManager.HandleCameraMovement(inputHandler);
            isInteracting = animatorManager.anim.GetBool("isInteracting");
            isJumping = animatorManager.anim.GetBool("isJumping");
            // animatorManager.anim.SetBool("Targeting", isTargeting);
            locomotion.HandleFallingAndLanding(isInteracting, isJumping, animatorManager);
            locomotion.HandleJumping(isInteracting, isJumping, inputHandler, animatorManager);
        }
    }
}
