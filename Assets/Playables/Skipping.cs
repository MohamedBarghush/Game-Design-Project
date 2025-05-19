using UnityEngine;
using UnityEngine.Playables;

public class CinematicSkipper : MonoBehaviour
{
    public PlayableDirector director;   // Assign your Timeline Director here
    public float skipToTime = 5.0f;     // Time in seconds to skip to
    public KeyCode skipKey = KeyCode.Space;

    private bool skipped = false;

    void LateUpdate()
    {
        if (!skipped && Input.GetKeyDown(skipKey))
        {
            SkipCinematic();
        }
    }

    void SkipCinematic()
    {
        if (director != null && director.state == PlayState.Playing)
        {
            director.time = skipToTime;
            // director.Evaluate(); // Forces the timeline to update immediately
            skipped = true;
            // Debug.Log($"Cinematic skipped to {skipToTime} seconds.");
        }
    }
}
