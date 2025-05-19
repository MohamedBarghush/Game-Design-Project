using UnityEngine;

public class CursorToggle : MonoBehaviour
{
    [Tooltip("Drag in the GameObject whose active state should control the cursor.")]
    public GameObject targetObj;

    void Update()
    {
        // Check whether the object is active (locally)
        bool isActive = targetObj.activeSelf;  // :contentReference[oaicite:0]{index=0}

        // Show the cursor if the object is active, hide it otherwise
        Cursor.visible   = isActive;            // :contentReference[oaicite:1]{index=1}
        Cursor.lockState = isActive 
            ? CursorLockMode.None              // free & visible
            : CursorLockMode.Locked;           // locked & hidden :contentReference[oaicite:2]{index=2}
    }
}
