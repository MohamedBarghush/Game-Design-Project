using UnityEngine;

namespace Player {
    public class PlayerTrolling : MonoBehaviour
    {
        public void HandleTrolling(InputHandler inputHandler, PlayerAnimatorManager playerAnimatorManager)
        {
            if (inputHandler.trollInput)
            {
                inputHandler.trollInput = false;
                playerAnimatorManager.anim.applyRootMotion = true;
                playerAnimatorManager.PlayTargetAnimationTrigger("Troll", true);
            }
        }
    }

}
