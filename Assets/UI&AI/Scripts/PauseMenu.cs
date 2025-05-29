using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Player;

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

    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;
    public float defaultSensitivity = 1.0f;
    public float minSensitivity = 0.1f;
    public float maxSensitivity = 10.0f;
    public float currentSensitivity;
    public CameraHandler cameraHandler;
    public Toggle invertYToggle;


    private bool isPaused = false;
    private bool wasTalkingToNPC = false;

    void Start()
    {
        // Setup button click listeners
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(LoadMainMenu);

        currentSensitivity = defaultSensitivity;
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;
            sensitivitySlider.value = currentSensitivity;
        }
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        invertYToggle.isOn = cameraHandler.invertY;
        invertYToggle.onValueChanged.AddListener(SetInvertY);
    }

    void SetSensitivity(float value)
    {
        currentSensitivity = value;
        if (cameraHandler != null)
        {
            cameraHandler.sensitivityX = value;
            cameraHandler.sensitivityY = value;
        }
    }

    void SetInvertY(bool isOn)
    {
        if (cameraHandler != null)
        {
            cameraHandler.invertY = isOn;
        }
    }

void Update()
{
    // Check if the GameObject is active in the hierarchy
    if (!gameObject.activeInHierarchy)
        return;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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