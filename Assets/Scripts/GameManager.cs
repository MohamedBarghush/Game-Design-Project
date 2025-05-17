using System.Collections.Generic;
using UnityEngine;

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
        QuestWriter.instance.StartTyping("Follow your wolf");
    }

    private void LateUpdate()
    {
        
    }

    public void OnEnemyKilled()
    {
        requiredEnemiesToKill--;
        if (requiredEnemiesToKill <= 0)
        {
            Phase2GO.ForEach(go => go.SetActive(true));
        }
    }

    public void OnPhase3Start()
    {
        Phase3GO.ForEach(go => go.SetActive(true));
    }
}
