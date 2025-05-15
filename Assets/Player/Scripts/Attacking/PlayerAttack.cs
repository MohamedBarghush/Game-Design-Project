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

        [Header("Attack Settings")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Transform enemyBackStabPos;
        [SerializeField] private float assassinationSphereRadius = 2.0f;
        [SerializeField] private Vector3 assassinationSphereOffset = new Vector3(0.7f, 1f, 0.2f);

        public void Awake() {
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        }

        public void HandleAttacking (PlayerLocomotion locomotion, InputHandler inputHandler, PlayerAnimatorManager playerAnimatorManager) {
            if (inputHandler.attackInput) {
                inputHandler.attackInput = false;
                if (canReAttack) {
                    // Security Measures
                    locomotion.ResetCrouching(playerAnimatorManager);

                    // Check for Assassination Possibility
                    Vector3 actualOffset = transform.position
                                         + (transform.forward * assassinationSphereOffset.x)
                                         + new Vector3(0, assassinationSphereOffset.y, 0) 
                                         + (transform.right * assassinationSphereOffset.z);

                    Collider[] enemyInVicinity = Physics.OverlapSphere(actualOffset, assassinationSphereRadius, enemyLayer);
                    if (enemyInVicinity.Length > 0) {
                        // Debug.Log("Assassination Possible");
                        if (enemyInVicinity[0].transform.TryGetComponent(out EnemyDefiner enemy)) {
                            if (enemy.CanBeAssassinated) {
                                // Debug.Log("Enemy cannot be assassinated
                                // Debug.Log("An Enemy");
                                Vector3 direction = enemyInVicinity[0].transform.transform.position - attackPoint.position;
                                direction.Normalize();
                                float dot = Vector3.Dot(direction, enemyInVicinity[0].transform.transform.forward);
                                // Debug.Log("Dot: " + dot);
                                if (dot > 0f) {
                                    transform.position = Vector3.Lerp(transform.position, enemyInVicinity[0].transform.position - enemyInVicinity[0].transform.forward * 0.5f, Time.deltaTime * 5f);
                                    playerAnimatorManager.anim.applyRootMotion = true;
                                    playerAnimatorManager.PlayTargetAnimationTrigger("Assassinate", true);
                                    enemy.Get_Assassinated(enemyBackStabPos);
                                    return;
                                }
                            }
                        }
                    }
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

        public void DamageEnemy() {
            Collider[] colliders = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider collider in colliders) {
                if (collider.TryGetComponent(out EnemyDefiner enemy)) {
                    enemy.TakeDamage(combo[attackIdx].damage);
                    // Debug.Log("Hit " + enemy.name);
                }
            }
        }

        private void OnDrawGizmosSelected() {
            if (attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);

            Vector3 actualOffset = transform.position
                                    + (transform.forward * assassinationSphereOffset.x)
                                    + new Vector3(0, assassinationSphereOffset.y, 0) 
                                    + (transform.right * assassinationSphereOffset.z);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(actualOffset, assassinationSphereRadius);
        }
    }
}