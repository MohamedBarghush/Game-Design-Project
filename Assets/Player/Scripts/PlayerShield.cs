using Player;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    private float parryTimer = 0.0f;
    [SerializeField] private float parryWindow = 0.2f;
    // [SerializeField] private bool isParrying = false;

    public void HandleShield(bool isJumping, bool isGrounded, bool isInteracting, InputHandler inputHandler, PlayerAnimatorManager animatorManager) {
        // Handle parrying
        if (parryTimer > 0) {
            // isParrying = true;
            parryTimer -= Time.deltaTime;
        } else {
            // isParrying = false;
        }

        if (inputHandler.blockInput)
        {
            inputHandler.blockInput = false;
            if (!isGrounded || isJumping || isInteracting) return;

            // isParrying = true;
            parryTimer = parryWindow;
            animatorManager.PlayTargetAnimation("Block", true);
            animatorManager.anim.applyRootMotion = true;
        }
    }
}
