using UnityEngine;

public class TutorialSaves : MonoBehaviour
{
    public GameObject keyPrefab;
    public string key;

    void OnTriggerEnter () {
        if (PlayerPrefs.HasKey(key))
        {
            Destroy(gameObject);
        } else {
            PlayerPrefs.SetInt(key, 1);
            FlashControl.instance.FlashUI(keyPrefab);
            Destroy(gameObject);
        }
    }
}
