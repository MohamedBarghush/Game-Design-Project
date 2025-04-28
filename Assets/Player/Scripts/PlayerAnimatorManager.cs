using UnityEngine;

namespace Player
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        int horizontal;
        int vertical;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            anim = GetComponent<Animator>();
            horizontal = Animator.StringToHash("Horizontal");
            vertical = Animator.StringToHash("Vertical");
        }

        public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
        {
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnimation, 0.2f);
        }

        public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            // Animation Snapping
            float snappedHorizontal;
            float snappedVertical;

            #region Snapped Vertical
            if (horizontalMovement > 0f && horizontalMovement < 0.55f) snappedHorizontal = 0.5f;
            else if (horizontalMovement > 0.55f) snappedHorizontal = 1f;
            else if (horizontalMovement < 0f && horizontalMovement > -0.55f) snappedHorizontal = -0.5f;
            else if (horizontalMovement < -0.55f) snappedHorizontal = -1f;
            else snappedHorizontal = 0f;
            #endregion
            #region Snapped Horizontal
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

            snappedVertical = Mathf.Abs(snappedVertical);

            anim.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
            anim.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
        }
    }
}
