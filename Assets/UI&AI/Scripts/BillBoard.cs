using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Tooltip("If true, the object will only rotate around its Y axis")]
    public bool lockYAxis = true;

    private Camera _cam;

    void Start()
    {
        // Cache the main camera
        _cam = Camera.main; 
        if (_cam == null)
            Debug.LogError("No camera tagged MainCamera found in scene.");
    }

    void LateUpdate()
    {
        if (_cam == null) return;

        // Get the forward direction of the camera
        Vector3 camForward = _cam.transform.forward;

        if (lockYAxis)
        {
            // zero out vertical component so it stays upright
            camForward.y = 0; 
            if (camForward.sqrMagnitude < 0.001f) return;
        }

        // Align this object's forward vector with the (possibly flattened) camera forward
        transform.rotation = Quaternion.LookRotation(camForward); 
    }
}
