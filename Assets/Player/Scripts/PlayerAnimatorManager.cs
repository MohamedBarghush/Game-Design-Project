using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        int horizontal;
        int vertical;

        RuntimeAnimatorController animatorOriginalController;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            anim = GetComponent<Animator>();
            horizontal = Animator.StringToHash("Horizontal");
            vertical = Animator.StringToHash("Vertical");
            animatorOriginalController = anim.runtimeAnimatorController;
        }

        // Specific function for playing uninterrupted animations using the isInteracting parameter handeled in the PlayerManager
        public void PlayTargetAnimation(string targetAnimation, bool isInteracting, float transitionDuration = 0.2f)
        {
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnimation, transitionDuration);
        }

        public void PlayTargetAnimationTrigger(string targetAnimation, bool isInteracting)
        {
            anim.SetBool("isInteracting", isInteracting);
            anim.SetTrigger(targetAnimation);
        }

        // Handling Movement and Sprinting animations
        public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            // Animation Snapping
            float snappedHorizontal;
            float snappedVertical;

            #region Snapped Horizontal
            if (horizontalMovement > 0f && horizontalMovement < 0.55f) snappedHorizontal = 0.5f;
            else if (horizontalMovement > 0.55f) snappedHorizontal = 1f;
            else if (horizontalMovement < 0f && horizontalMovement > -0.55f) snappedHorizontal = -0.5f;
            else if (horizontalMovement < -0.55f) snappedHorizontal = -1f;
            else snappedHorizontal = 0f;
            #endregion
            #region Snapped Vertical
            if (verticalMovement > 0f && verticalMovement < 0.55f) snappedVertical = 0.5f;
            else if (verticalMovement > 0.55f) snappedVertical = 1f;
            else if (verticalMovement < 0f && verticalMovement > -0.55f) snappedVertical = -0.5f;
            else if (verticalMovement < -0.55f) snappedVertical = -1f;
            else snappedVertical = 0f;
            #endregion

            if (isSprinting)
            {
                snappedHorizontal = horizontalMovement;
                snappedVertical = 2f;
            }

            // snappedVertical = horizontalMovement == 0 ? Mathf.Abs(snappedVertical) : snappedVertical;

            anim.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
            anim.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
        }

        // public void ResetAnimatorOverrideController() {
        //     anim.runtimeAnimatorController = animatorOriginalController;
        // }

        // public void OverlayLayer(int layerIndex, float layerWeight, string animationName, bool animationStatus)
        // {
        //     anim.SetLayerWeight(layerIndex, layerWeight);
        //     anim.SetBool(animationName, animationStatus);
        // }
    }
}
