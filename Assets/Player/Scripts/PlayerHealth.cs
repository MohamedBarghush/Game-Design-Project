using UnityEngine;
using System.Collections;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        PlayerAnimatorManager animatorManager;
        PlayerManager playerManager;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 0;
        [SerializeField] private HealthBar healthBar;

        // private Coroutine regenCoroutine;
        // private Coroutine regenDelayCoroutine;
        // private bool isRegenerating = false;

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

            // if (regenCoroutine != null)
            //     StopCoroutine(regenCoroutine);
            // if (regenDelayCoroutine != null)
            //     StopCoroutine(regenDelayCoroutine);

            // isRegenerating = false;
            // regenDelayCoroutine = StartCoroutine(StartRegenAfterDelay(10f));

            if (currentHealth <= 0)
            {
                animatorManager.PlayTargetAnimationTrigger("Death", true);
                playerManager.isDead = true;
                GameManager.instance.EndGame();
                return;
            }

            animatorManager.PlayTargetAnimationTrigger("TakeDamage", true);
            playerManager.isInvulnerable = true;
            healthBar?.SetHealth(currentHealth);
            StartCoroutine(ResetInvulnerabilityAfterDelay(1.0f));
        }

        public void GetGrapped(int damage)
        {
            if (playerManager.isInvulnerable || playerManager.isDead) return;
            currentHealth -= damage;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            GetComponent<Collider>().isTrigger = true;
            animatorManager.anim.applyRootMotion = true;

            // transform.position = grabberTransform.position + new Vector3(0, 1.5f, 0); // Adjust position to be in front of the grabber

            // if (regenCoroutine != null)
            //     StopCoroutine(regenCoroutine);
            // if (regenDelayCoroutine != null)
            //     StopCoroutine(regenDelayCoroutine);

            // isRegenerating = false;
            // regenDelayCoroutine = StartCoroutine(StartRegenAfterDelay(10f));

            if (currentHealth <= 0)
            {
                animatorManager.PlayTargetAnimationTrigger("Death", true);
                playerManager.isDead = true;
                GameManager.instance.EndGame();
                return;
            }
            animatorManager.PlayTargetAnimation("Grabbed", true, 0.0f);
            playerManager.isInvulnerable = true;
            StartCoroutine(ResetViable(3.0f));
            StartCoroutine(ResetInvulnerabilityAfterDelay(3.0f));
        }

        // public void Heal(int amount)
        // {
        //     if (playerManager.isDead) return;
        //     currentHealth += amount;
        //     if (currentHealth > maxHealth)
        //     {
        //         currentHealth = maxHealth;
        //     }

        // }

        public void HealthHandler()
        {
            // Reserved for future expansion
        }

        private IEnumerator ResetInvulnerabilityAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerManager.isInvulnerable = false;
        }

        private IEnumerator ResetViable(float delay)
        {
            yield return new WaitForSeconds(delay);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Collider>().isTrigger = false;
        }

        // private IEnumerator StartRegenAfterDelay(float delay)
        // {
        //     yield return new WaitForSeconds(delay);
        //     regenCoroutine = StartCoroutine(RegenerateHealthOverTime());
        // }

        // private IEnumerator RegenerateHealthOverTime()
        // {
        //     isRegenerating = true;
        //     while (currentHealth < maxHealth)
        //     {
        //         currentHealth += 1; // Heal 1 point per tick
        //         if (currentHealth > maxHealth)
        //             currentHealth = maxHealth;

        //         yield return new WaitForSeconds(0.1f); // Slow regeneration rate
        //     }
        //     isRegenerating = false;
        // }
    }
}
