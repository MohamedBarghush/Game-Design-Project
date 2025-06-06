using UnityEngine;

public class DangerNoti : MonoBehaviour
{
    public static DangerNoti instance;
    Animator anim;

    void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void InvokeNoti() {
        AudioManager.Instance.PlaySound(SoundType.Danger);
        anim.SetTrigger("Danger");
    }
}
