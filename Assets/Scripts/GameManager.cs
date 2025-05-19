using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int requiredEnemiesToKill = 8;

    public List<GameObject> Phase2GO;
    public List<GameObject> Phase3GO;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(SoundType.BG);
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

    public void EndGame(){
        StartCoroutine(EndGameCoroutine());
    }

    private System.Collections.IEnumerator EndGameCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
