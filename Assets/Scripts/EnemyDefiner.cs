using UnityEngine;

public abstract class EnemyDefiner : MonoBehaviour
{
    public virtual bool CanBeAssassinated { get; private set; }
    public virtual bool isVulnerable { get; private set; }

    public virtual void SetIsVulnerable(bool state)
    {
        isVulnerable = state;
    }

    public virtual void SetAssassinationState(bool state)
    {
        CanBeAssassinated = state;
    }

    public abstract void TakeDamage(int damage);

    public virtual void Get_Assassinated(Transform enemyBackStabPos) { }
}
