using UnityEngine;

public class MinimapBlib : MonoBehaviour
{
    public Transform target;
    public float yOffset = 10f;
    public float followSpeed = 5f;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + new Vector3(0, yOffset, 0);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
}

