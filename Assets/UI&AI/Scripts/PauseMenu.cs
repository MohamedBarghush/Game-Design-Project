using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hudCanvas;
    public GameObject pauseMenuPanel;
    public Button resumeButton;    // Drag your Resume Button in Inspector
    public Button mainMenuButton;  // Drag your Main Menu Button in Inspector

    [Header("Dependencies")]
    public InGameUI inGameUI;
    public PlayerInput playerInput;
    public MonoBehaviour cameraController;

    private bool isPaused = false;
    private bool wasTalkingToNPC = false;

    void Update()
    {
        // Handle ESC key for pause/unpause
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !inGameUI.isTalkingToNPC)
        {
            TogglePause();
        }

        // Direct button checking when paused
        if (isPaused)
        {
            CheckButtonPresses();
        }
    }

    void CheckButtonPresses()
    {
        // Check Resume Button
        if (IsButtonPressed(resumeButton))
        {
            HandleResume();
        }

        // Check Main Menu Button
        if (IsButtonPressed(mainMenuButton))
        {
            HandleMainMenu();
        }
    }

    bool IsButtonPressed(Button button)
    {
        // Check if mouse is over the button and left mouse button is clicked
        return RectTransformUtility.RectangleContainsScreenPoint(
            button.GetComponent<RectTransform>(), 
            Mouse.current.position.ReadValue(), 
            null
        ) && Mouse.current.leftButton.wasPressedThisFrame;
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        hudCanvas.SetActive(!isPaused);
        pauseMenuPanel.SetActive(isPaused);
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        if (playerInput != null) playerInput.enabled = !isPaused;
        if (cameraController != null) cameraController.enabled = !isPaused;
    }

    void HandleResume()
    {
        TogglePause();
    }

public void HandleMainMenu()
{
    Time.timeScale = 1f;
    
    // Destroy persistent objects that might hold scene references
    if (hudCanvas != null) Destroy(hudCanvas);
    if (pauseMenuPanel != null) Destroy(pauseMenuPanel);
    
    SceneManager.LoadScene("MainMenu");
}
}