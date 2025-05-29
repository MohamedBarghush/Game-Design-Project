using UnityEngine;

public abstract class SpellBase : ScriptableObject
{
    public string spellName;
    public int animationID;
    public float cooldown;
    public float staminaCost;
    public float castTime = 2.0f;
    public LayerMask playerLayer;
    public GameObject castEffect;

    protected Transform castPoint;
    protected Transform playerTarget;
    protected Vector3 startPosition;

    protected AudioSource audioSource;

    public virtual void Initialize(Transform castPoint, Transform player, Vector3 startPosition, AudioSource audioSource = null)
    {
        this.castPoint = castPoint;
        this.startPosition = startPosition;
        this.audioSource = audioSource;
        playerTarget = player;
    }

    public abstract void Cast();

    public virtual float GetTotalDuration()
    {
        return castTime; // Default implementation for other spells
    }
}