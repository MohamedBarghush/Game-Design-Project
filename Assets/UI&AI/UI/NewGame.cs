using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{
    public Button newGameButton; // Assign your button in the Inspector
    public string sceneName = "Level 1"; // Name of your scene

    void Start()
    {
        newGameButton.onClick.AddListener(StartNewGame);
    }

    void StartNewGame()
    {
        SceneManager.LoadScene(sceneName); // Direct scene load
    }
}