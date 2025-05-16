using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public SoundLibrary soundLibrary;
    public AudioSource soundEffectSource;
    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundType soundType)
    {
        AudioClip clip = soundLibrary.GetRandomClip(soundType);
        if (clip != null)
        {
            soundEffectSource.PlayOneShot(clip);
        }
        
    }
   

    public void PlayMusic(SoundType soundType)
    {
        AudioClip clip = soundLibrary.GetRandomClip(soundType);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
