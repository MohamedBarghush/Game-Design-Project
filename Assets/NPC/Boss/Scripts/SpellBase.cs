using UnityEngine;

public abstract class SpellBase : ScriptableObject
{
    public string spellName;
    public int animationID;
    public float cooldown;
    public float staminaCost;
    public LayerMask playerLayer;
    public GameObject castEffect;

    protected Transform castPoint;
    protected Transform playerTarget;
    protected Vector3 startPosition;

    public virtual void Initialize(Transform castPoint, Transform player, Vector3 startPosition)
    {
        this.castPoint = castPoint;
        this.startPosition = startPosition;
        playerTarget = player;
    }

    public abstract void Cast();

    public virtual float GetTotalDuration()
    {
        return 0; // Default implementation for other spells
    }
}