using UnityEngine;
using UnityEngine.InputSystem;

public class DisablePlayerInputBehaviour : MonoBehaviour
{
    [Tooltip("The GameObject whose active state drives input blocking")]
    public GameObject cinematicObject;

    [Tooltip("Your PlayerInput (New Input System) to disable/enable")]
    public PlayerInput playerInput;

    private bool hasToggledInput = false;

    void Update()
    {
        if (cinematicObject == null || playerInput == null)
            return;

        // Disable input when cinematicObject is active
        if (cinematicObject.activeInHierarchy && !hasToggledInput)
        {
            if (playerInput.enabled)
            {
                playerInput.enabled = false;
                hasToggledInput = true; // Mark as toggled
            }
        }
        // Re-enable input when cinematicObject is inactive
        else if (!cinematicObject.activeInHierarchy && hasToggledInput)
        {
            if (!playerInput.enabled)
            {
                playerInput.enabled = true;
                hasToggledInput = false; // Reset toggle for future use
            }
        }
    }
}