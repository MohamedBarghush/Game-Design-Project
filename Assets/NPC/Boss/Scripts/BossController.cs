using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossController : EnemyDefiner
{
    public static BossController Instance { get; private set; }

    [Header("State Machine")]
    [SerializeField] private BossState currentState;
    private Dictionary<BossState, IState> states;

    [Header("Combat")]
    [SerializeField] private float maxHealth = 1000;
    [SerializeField] private float stunThreshold = 30;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentStamina;
    [SerializeField] private Vector3 floatingPosition;
    // [SerializeField] private float staminaRecoveryRate = 5f;

    [Header("References")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<SpellBase> spells = new List<SpellBase>();

    private Dictionary<SpellBase, float> spellCooldowns = new Dictionary<SpellBase, float>();
    private bool isFloating = true;
        
    public List<GameObject> flyingShields = new List<GameObject>();

    public bool isCasting = false;
    private Coroutine currentSpellRoutine;

    // public bool IsCasting => isCasting;

    private void Awake()
    {
        Instance = this;
        InitializeStates();
        currentHealth = maxHealth;
        currentStamina = stunThreshold;
        rb.isKinematic = true;
        animator.SetBool("Floating", true);
        floatingPosition = transform.position;

        foreach (SpellBase spell in spells)
        {
            spell.Initialize(castPoint, player, transform.position);
            spellCooldowns.Add(spell, 0);
        }

        SetIsVulnerable(true);
        SetAssassinationState(false);
    }

    public void EnableShield() {
        Instantiate(flyingShields[Random.Range(0, flyingShields.Count)], transform.position, Quaternion.identity, transform);
    }

    private void InitializeStates()
    {
        states = new Dictionary<BossState, IState>
        {
            { BossState.Idle, new IdleState(this) },
            { BossState.Attacking, new AttackState(this) },
            { BossState.Stunned, new StunnedState(this) }
        };
    }

    private void Update()
    {
        states[currentState].UpdateState();
        UpdateCooldowns();
        HandleStateTransitions();
        UpdateLookAtPlayer();
    }

    private void UpdateLookAtPlayer()
    {
        if (isFloating)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Keep the y-axis unchanged
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    private void UpdateCooldowns()
    {
        List<SpellBase> keys = new List<SpellBase>(spellCooldowns.Keys);
        foreach (SpellBase spell in keys)
        {
            if (spellCooldowns[spell] > 0)
                spellCooldowns[spell] -= Time.deltaTime;
            // spellCooldownText.text += $"{spell.name}: {spellCooldowns[spell]:0.0}\n";
        }
    }

    private void HandleStateTransitions()
    {
        // Modified to check for stamina recovery needs
        if (currentStamina <= stunThreshold * 0.3f && 
            currentState != BossState.Stunned &&
            HasAvailableStaminaRecovery())
        {
            ChangeState(BossState.Attacking);
        }
        else if (currentStamina <= 0 && currentState != BossState.Stunned)
        {
            ChangeState(BossState.Stunned);
        }
    }

    private bool HasAvailableStaminaRecovery()
    {
        StaminaSpell staminaSpell = (StaminaSpell)spells.Find(s => s is StaminaSpell);
        return staminaSpell != null && 
            spellCooldowns[staminaSpell] <= 0 && 
            currentStamina > staminaSpell.staminaCost;
    }

    public void ChangeState(BossState newState)
    {
        states[currentState].ExitState();
        currentState = newState;
        states[currentState].EnterState();
    }

    public void TriggerAttackAnimation(int spellID)
    {
        animator.SetInteger("SpellID", spellID);
        animator.SetTrigger("Attack");
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentStamina -= damage * 2;
        AudioManager.Instance.PlaySound(SoundType.Hit_Boss);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.SetTrigger("Die");
            GameManager.instance.EndGame();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
        Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void RecoverHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void RecoverStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, stunThreshold);
    }

    public SpellBase GetAvailableSpell()
    {
        List<SpellBase> availableSpells = new List<SpellBase>();
        SpellBase healSpell = spells.Find(s => s is HealSpell);
        SpellBase staminaSpell = spells.Find(s => s is StaminaSpell);

        if (currentHealth < maxHealth * 0.3f && 
            healSpell != null && 
            spellCooldowns[healSpell] <= 0 && 
            currentStamina > healSpell.staminaCost)
        {
            spellCooldowns[healSpell] = healSpell.cooldown;
            return healSpell;
        }

        // Priority 2: Stamina Recovery (more aggressive threshold)
        if (currentStamina < stunThreshold * 0.25f && 
            staminaSpell != null && 
            spellCooldowns[staminaSpell] <= 0 && 
            currentStamina > staminaSpell.staminaCost)
        {
            spellCooldowns[staminaSpell] = staminaSpell.cooldown;
            return staminaSpell;
        }

        // Regular spells
        foreach (SpellBase spell in spells)
        {
            if (spell is HealSpell || spell is StaminaSpell) continue;
            
            if (spellCooldowns[spell] <= 0 && currentStamina >= spell.staminaCost)
            {
                availableSpells.Add(spell);
            }
        }


        SpellBase randomSpell = availableSpells.Count > 0 ? availableSpells[Random.Range(0, availableSpells.Count)] : null;
        if (randomSpell != null)
        {
            spellCooldowns[randomSpell] = randomSpell.cooldown;
        }
        return randomSpell;
        // return availableSpells.Count > 0 ? availableSpells[Random.Range(0, availableSpells.Count)] : null;
    }

    public bool IsAnimationDone()
    {
        return animator.GetCurrentAnimatorStateInfo(2).IsName("New State");
    }

    // Called by animation events
    public void CastCurrentSpell(int spellID)
    {
        if (isCasting) return;
    
        SpellBase spell = spells.Find(s => s.animationID == spellID);
        if (spell != null)
        {
            isCasting = true;
            currentSpellRoutine = StartCoroutine(SpellCastRoutine(spell));
        }
    }

    private IEnumerator SpellCastRoutine(SpellBase spell)
    {
        spell.Cast();
        spellCooldowns[spell] = spell.cooldown;
        currentStamina -= spell.staminaCost;

        yield return new WaitForSeconds(spell.GetTotalDuration());
        
        isCasting = false;
    
    }
    public void FinishSpell()
    {
        if (currentSpellRoutine != null)
        {
            StopCoroutine(currentSpellRoutine);
        }
        isCasting = false;
    }

    public void FallToGround()
    {
        isFloating = false;
        rb.isKinematic = false;
        animator.SetBool("Floating", false);
    }

    public void RecoverFromStun()
    {
        isFloating = true;
        rb.isKinematic = true;
        animator.SetBool("Floating", true);
        EnableShield();
        StartCoroutine(GetBackToFloating());
        currentStamina = stunThreshold;
    }

    private IEnumerator GetBackToFloating()
    {
        SetIsVulnerable(false);
        EnableShield();
        yield return new WaitForSeconds(1f);
        while (Vector3.Distance(transform.position, floatingPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, floatingPosition, Time.deltaTime * 2);
            yield return null;
        }
        SetIsVulnerable(true);
    }

}