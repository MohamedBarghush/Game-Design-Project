using System.Collections.Generic;
using UnityEngine;

namespace Player 
{
    public class PlayerAttack : MonoBehaviour
    {

        [SerializeField] private List<AttackOverrideSO> combo;
        [SerializeField] private int attackIdx = 0;
        [SerializeField] private int attackAnimationHash = 0;
        [SerializeField] private float currentTimer = 0f;
        [SerializeField] private bool canReAttack = true;
        // [SerializeField] public bool attacking = false;
        PlayerAnimatorManager playerAnimatorManager;

        public void Awake() {
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        }


        public void HandleAttacking (PlayerLocomotion locomotion, InputHandler inputHandler, PlayerAnimatorManager playerAnimatorManager) {
            if (inputHandler.attackInput) {
                inputHandler.attackInput = false;
                if (canReAttack) {
                    // Security Measures
                    locomotion.ResetCrouching(playerAnimatorManager);

                    // playerAnimatorManager.PlayTargetAnimation("Attack", true);
                    playerAnimatorManager.anim.SetBool("isInteracting", true);
                    playerAnimatorManager.anim.SetTrigger("Attack");
                    playerAnimatorManager.anim.applyRootMotion = true;
                    // playerAnimatorManager.anim.runtimeAnimatorController = combo[attackIdx].animatorOverrideController;
                    // playerAnimatorManager.anim.SetInteger("AttackIdx", attackIdx);
                    // playerAnimatorManager.anim.Play("Attack", 2, 0f);
                    currentTimer = combo[attackIdx].exitTime;
                    attackIdx = (attackIdx + 1) % combo.Count;
                    attackAnimationHash = (attackAnimationHash + 1) % 2;
                    canReAttack = false;
                    // attacking = true;
                }
            }

            // if (upcomingAttack) {
            //     // upcomingAttack = false;
                
            // }
            if (currentTimer <= -0.5f) {
                // currentTimer = 0f;
                canReAttack = true;
                // attacking = false;
            } else {
                currentTimer -= Time.deltaTime;
            }
        }

        public void ReEnableAttack() {
            canReAttack = true;
            playerAnimatorManager.anim.SetBool("isInteracting", false);
        }
    }
}