using UnityEngine;
public enum SoundType
{
    BG,
    Duck,
    Barrel,
    Book,
    Shoot,
    Hook,
    Bottel,
    Chair,
    Frog,
    Cat,
    Bear,
    TryAgain,
    AcquireWeapon
}
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]

public class SoundLibrary : ScriptableObject
{
    
    [System.Serializable]
    public class Sound
    {
        public SoundType soundType;
        public AudioClip[] clips; 
    }

    public Sound[] sounds;

    public AudioClip GetRandomClip(SoundType soundType)
    {
        foreach (var sound in sounds)
        {
            if (sound.soundType == soundType && sound.clips.Length > 0)
            {
                return sound.clips[Random.Range(0, sound.clips.Length)];
            }
        }
        return null;
    }
}
