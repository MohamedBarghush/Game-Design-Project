using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class WolfInteractDisabler : MonoBehaviour
{
    [Tooltip("The Animator controlling your wolf")]
    public Animator wolfAnimator;

    [Tooltip("Your PlayerInput component (New Input System)")]
    public PlayerInput playerInput;

    [Tooltip("Name of the action you want to disable/enable (e.g. \"Interact\")")]
    public string actionName = "Interact";

    [Tooltip("The UI Image to hide/show during running")]
    public Image uiImage;

    [Tooltip("The TextMeshProUGUI text to hide/show during running")]
    public TMP_Text uiText;

    // cached hash & action
    private int runStateHash;
    private InputAction interactAction;

    void Start()
    {
        // Cache the hash for the "Run" state on layer 0
        runStateHash = Animator.StringToHash("run");

        if (playerInput == null)
        {
            Debug.LogError("WolfInteractDisabler: PlayerInput not assigned.");
            enabled = false;
            return;
        }

        interactAction = playerInput.actions.FindAction(actionName);
        if (interactAction == null)
        {
            Debug.LogError($"WolfInteractDisabler: Action '{actionName}' not found.");
            enabled = false;
        }

        if (uiImage == null)
            Debug.LogWarning("WolfInteractDisabler: uiImage not assigned; skipping image toggle.");

        if (uiText == null)
            Debug.LogWarning("WolfInteractDisabler: uiText not assigned; skipping text toggle.");
    }

    void Update()
    {
        if (wolfAnimator == null) return;

        // Check run state
        bool isRunning = wolfAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == runStateHash;

        // Toggle the interact action
        if (interactAction != null)
        {
            if (isRunning && interactAction.enabled)
                interactAction.Disable();
            else if (!isRunning && !interactAction.enabled)
                interactAction.Enable();
        }

        // Toggle UI Image visibility
        if (uiImage != null)
            uiImage.enabled = !isRunning;

        // Toggle TMP Text visibility
        if (uiText != null)
            uiText.enabled = !isRunning;
    }
}
