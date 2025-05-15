// IdleState.cs
using UnityEngine;

public class IdleState : IState
{
    private BossController boss;
    private float idleTimer;

    public IdleState(BossController bossController)
    {
        boss = bossController;
    }

    public void EnterState()
    {
        idleTimer = Random.Range(1f, 3f);
    }

    public void UpdateState()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            boss.ChangeState(BossState.Attacking);
        }
    }

    public void ExitState() { }
}

// AttackState.cs

public class AttackState : IState
{
    private BossController boss;
    private SpellBase currentSpell;

    public AttackState(BossController bossController)
    {
        boss = bossController;
    }

    public void EnterState()
    {
        currentSpell = boss.GetAvailableSpell();
        if (currentSpell != null)
        {
            boss.TriggerAttackAnimation(currentSpell.animationID);
        }
        else
        {
            boss.ChangeState(BossState.Idle);
        }
    }

    public void UpdateState()
    {
        if (!boss.isCasting && boss.IsAnimationDone())
        {
            boss.ChangeState(BossState.Idle);
        }
    }

    public void ExitState() { }
}

// StunnedState.cs

public class StunnedState : IState
{
    private BossController boss;
    private float stunDuration = 15f;
    private float timer;

    public StunnedState(BossController bossController)
    {
        boss = bossController;
    }

    public void EnterState()
    {
        timer = stunDuration;
        boss.FallToGround();
    }

    public void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            boss.ChangeState(BossState.Idle);
        }
    }

    public void ExitState()
    {
        boss.RecoverFromStun();
    }
}