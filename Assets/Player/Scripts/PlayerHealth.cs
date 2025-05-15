using Player;
using UnityEngine;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        PlayerAnimatorManager animatorManager;
        PlayerManager playerManager;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 0;

        public void HealthAwake(PlayerAnimatorManager animatorManager)
        {
            this.animatorManager = animatorManager;
            playerManager = GetComponent<PlayerManager>();
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (playerManager.isInvulnerable || playerManager.isDead) return;
            currentHealth -= damage;
            animatorManager.anim.applyRootMotion = true;
            if (currentHealth <= 0) {
                animatorManager.PlayTargetAnimationTrigger("Death", true);
                playerManager.isDead = true;
                return;
            }
            animatorManager.PlayTargetAnimationTrigger("TakeDamage", true);
            playerManager.isInvulnerable = true;
            StartCoroutine(ResetInvulnerabilityAfterDelay(1.0f));
        }

        public void GetGrapped(int damage)
        {
            if (playerManager.isInvulnerable || playerManager.isDead) return;
            currentHealth -= damage;
            animatorManager.anim.applyRootMotion = true;
            if (currentHealth <= 0) {
                animatorManager.PlayTargetAnimationTrigger("Death", true);
                playerManager.isDead = true;
                return;
            }
            animatorManager.PlayTargetAnimation("Grabbed", true, 0.0f);
            playerManager.isInvulnerable = true;
            StartCoroutine(ResetInvulnerabilityAfterDelay(1.0f));
        }

        public void Heal(int amount)
        {
            if (playerManager.isDead) return;
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        public void HealthHandler () {
            
        }

        private System.Collections.IEnumerator ResetInvulnerabilityAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerManager.isInvulnerable = false;
        }
    }
}
