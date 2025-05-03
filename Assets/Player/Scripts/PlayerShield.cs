using Player;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private bool blockInput;
    private float parryTimer = 0.0f;
    [SerializeField] private float parryWindow = 0.2f;
    [SerializeField] private bool isParrying = false;

    public void HandleShield(ref bool isBlocking, InputHandler inputHandler, PlayerAnimatorManager animatorManager) {
        blockInput = inputHandler.blockInput;
        animatorManager.anim.SetBool("isBlocking", isBlocking);
        if (blockInput) {
            if (!isBlocking) {
                isBlocking = true;
                parryTimer = parryWindow;
                animatorManager.PlayTargetAnimation("Block", true);
                animatorManager.anim.applyRootMotion = true;
            }
        } else {
            if (isBlocking) {
                isBlocking = false;
            }
        }

        // Handle parrying
        if (parryTimer > 0) {
            isParrying = true;
            parryTimer -= Time.deltaTime;
        } else {
            isParrying = false;
        }
    }
}
