using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource BackgroundMusic; // Optional
    public AudioSource SoundEffects;    // Optional

    void Awake()
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

    // Play background music (if assigned)
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (BackgroundMusic == null) return; // Skip if no AudioSource
        
        BackgroundMusic.clip = clip;
        BackgroundMusic.loop = true;
        BackgroundMusic.Play();
    }

    // Play sound effect (if assigned)
    public void PlaySoundEffect(AudioClip clip)
    {
        if (SoundEffects == null) return; // Skip if no AudioSource
        
        SoundEffects.PlayOneShot(clip);
    }

    // Optional: Stop background music
    public void StopBackgroundMusic()
    {
        if (BackgroundMusic != null && BackgroundMusic.isPlaying)
        {
            BackgroundMusic.Stop();
        }
    }
}