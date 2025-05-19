using UnityEngine;
using System.Collections;

namespace Player
{
    public class PlayerAuraManager : MonoBehaviour
    {
        private PlayerAnimatorManager animatorManager;
        private InputHandler inputHandler;

        public bool auraActive = false;
        [SerializeField] private GameObject auraVFX;

        // [SerializeField] private int auraLevel = 0; // 0 = no aura, 1 = basic aura, 2 = advanced aura, etc.


        [HideInInspector] public bool canStartAura = true; // Flag to control aura activation


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void PlayerAuraStart(PlayerAnimatorManager animatorManager, InputHandler inputHandler)
        {
            this.inputHandler = inputHandler;
            this.animatorManager = animatorManager;
            auraVFX.SetActive(false); // Ensure the aura VFX is initially inactive
        }

        public void HandleAura(bool isInteracting)
        {
            if (inputHandler.auraInput)
            {
                inputHandler.auraInput = false; // Reset the input to prevent toggling multiple times in one frame
                if (!canStartAura || isInteracting) return; // Prevent aura activation if already active

                auraActive = !auraActive;
                if (auraActive == true)
                {
                    AudioManager.Instance.PlaySound(SoundType.Goku, 1.0f);
                    animatorManager.PlayTargetAnimation("PowerUp", true);
                    auraVFX.SetActive(true);
                }
                else
                {
                    // Start a coroutine to disable the aura with delay
                    StartCoroutine(DisableAuraWithDelay());
                }
            }
        }

        private IEnumerator DisableAuraWithDelay()
        {
            canStartAura = false;
            // Stop all particle systems in the aura effect's children
            ParticleSystem[] particleSystems = auraVFX.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            // Wait for a short delay before disabling the aura effect
            yield return new WaitForSeconds(1f);

            // Disable the aura VFX
            auraVFX.SetActive(false);
            canStartAura = true; // Allow aura activation again
        }
    }
}
