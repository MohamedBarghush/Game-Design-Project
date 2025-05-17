using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hudCanvas;
    public GameObject pauseMenuPanel;

    [Header("UI Buttons")]
    public Button resumeButton;
    public Button mainMenuButton;

    [Header("Dependencies")]
    public InGameUI inGameUI;
    public PlayerInput playerInput;
    public MonoBehaviour cameraController;

    private bool isPaused = false;
    private bool wasTalkingToNPC = false;

    void Start()
    {
        // Setup button click listeners
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    void Update()
    {
        bool currentTalkingState = inGameUI.isTalkingToNPC;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!currentTalkingState && !wasTalkingToNPC)
            {
                TogglePause();
            }
        }

        wasTalkingToNPC = currentTalkingState;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;
        hudCanvas.SetActive(!isPaused);
        pauseMenuPanel.SetActive(isPaused);

        if (playerInput != null)
            playerInput.enabled = !isPaused;

        if (cameraController != null)
            cameraController.enabled = !isPaused;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    // Button Functions
    public void ResumeGame()
    {
        TogglePause();
    }

    public void LoadMainMenu()
    {
        // Reset time scale and cursor state before loading main menu
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}