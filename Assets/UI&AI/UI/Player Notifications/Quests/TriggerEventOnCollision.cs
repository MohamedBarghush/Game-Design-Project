using UnityEngine;
using UnityEngine.Events;

public class TriggerEventOnCollision : MonoBehaviour
{
    public UnityEvent eventOnTiggerEnter;
    void OnTriggerEnter(Collider other)
    {
        eventOnTiggerEnter.Invoke();
    }
}
