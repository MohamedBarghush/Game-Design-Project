using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{
    public string sceneName = "Level 1"; // Name of your scene

    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll(); // Clear all PlayerPrefs
        PlayerPrefs.Save();
    }

    public void ContinueGame()
    {
        // Implement your continue game logic here
    }
}