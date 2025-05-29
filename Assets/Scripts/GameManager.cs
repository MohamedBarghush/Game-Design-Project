using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int requiredEnemiesToKill = 8;

    public List<GameObject> Phase2GO;
    public List<GameObject> Phase3GO;

    public CanvasGroup endGameCanvasGroup;
    public TMP_Text textComponent;
    [SerializeField] private float typingSpeed = 0.05f;
    public Texture2D cursorTexture;
    public Vector2 cursorHotspot = new Vector2(64, 64);

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(SoundType.BG);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    private void LateUpdate()
    {
        
    }

    public void OnEnemyKilled()
    {
        // Debug.Log("Enemy killed somwhere");
        requiredEnemiesToKill--;
        if (requiredEnemiesToKill <= 0)
        {
            QuestWriter.instance.StartTyping("Find out what happened from the locals");
            Phase2GO.ForEach(go => go.SetActive(true));
            WolfManager.instance.OnVillageCleared();
        }
    }

    public void OnPhase3Start()
    {
        Phase3GO.ForEach(go => go.SetActive(true));
    }

    public void EndGame(String message = "You have DIED!\nExiting..."){
        StartCoroutine(EndGameCoroutine(message));
    }

    private IEnumerator EndGameCoroutine(String message)
    {
        endGameCanvasGroup.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        while (endGameCanvasGroup.alpha < 1f)
        {
            endGameCanvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
        StartTyping(message);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void StartTyping(string textToType)
    {
        StartCoroutine(TypeText(textToType));
    }

    private IEnumerator TypeText(string textToType)
    {
        textComponent.text = "";
        foreach (char c in textToType)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
