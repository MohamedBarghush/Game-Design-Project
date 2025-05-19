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
            AudioManager.Instance.PlaySound(SoundType.Tutorial, 0.3f);
            Destroy(gameObject);
        }
    }
}
