using UnityEngine;
using TMPro;

public class TextChangeAudio : MonoBehaviour
{
    [Tooltip("The TextMeshPro Text component to watch")]
    public TMP_Text watchedText;

    [Tooltip("The AudioClip to play when the text changes")]
    public AudioClip typingClip;

    [Tooltip("How many seconds to wait after last change before stopping audio")]
    public float stopDelay = 0.5f;

    private AudioSource audioSource;
    private string lastText;
    private float lastChangeTime;

    void Awake()
    {
        // Try to get an existing AudioSource, or add one if none exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configure it
        audioSource.clip = typingClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Initialize text tracking
        lastText = watchedText.text;
        lastChangeTime = Time.time;
    }

    void Update()
    {
        // If the text has changed, play (or keep playing) the clip
        if (watchedText.text != lastText)
        {
            lastText = watchedText.text;
            lastChangeTime = Time.time;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        // If it’s been quiet for longer than stopDelay, stop the clip
        else if (audioSource.isPlaying && Time.time - lastChangeTime > stopDelay)
        {
            audioSource.Stop();
        }
    }
}
