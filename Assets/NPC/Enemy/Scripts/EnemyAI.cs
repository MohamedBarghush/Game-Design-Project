using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Player;

public class EnemyAI : EnemyDefiner
{
    // States
    public enum State
    {
        Unaware,
        Follow,
        AwaitAttack,
        Attack,
        Die
    }
    public State currentState = State.Unaware;

    [Header("References")]
    public Transform player;
    public Transform idlePoint;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Detection & Ranges")]
    public float detectionRange = 15f;       // How far the enemy can “see”
    public float followDistance = 10f;       // When to stop following and start orbiting
    public float attackDistance = 3f;        // When attack can begin

    [Header("Orbiting")]
    public float orbitRadius = 8f;           // Radius of the hover circle
    public float orbitSpeed = 60f;           // Degrees per second
    private float currentOrbitAngle = 0f;
    public int orbitDirection = 1;           // 1 = clockwise, -1 = counterclockwise

    [Header("Attack")]
    [Range(0f,1f)] public float attackChancePerSecond = 0.2f; 
    public float attackDuration = 1f;        // Length of attack animation
    private float orbitAttackDelayTimer = 0f; // Timer for attack delay

    [Header("Animation Parameters")]
    private static readonly int AnimFollow   = Animator.StringToHash("Follow");
    private static readonly int AnimAttack   = Animator.StringToHash("Attack");
    private static readonly int AnimOrbit    = Animator.StringToHash("Orbit");
    private static readonly int AnimOrbiting = Animator.StringToHash("Orbiting");
    private static readonly int AnimHit      = Animator.StringToHash("Hit");
    private static readonly int AnimDie      = Animator.StringToHash("Death");
    private static readonly int AnimBackStab = Animator.StringToHash("BackStab");

    [Header("Health")]
    public int health = 100;

    [Header("Damaging Parameters")]
    [SerializeField] private int damage = 10; // Damage dealt to player
    [SerializeField] private Transform damageCenter;
    [SerializeField] private float damageRadius = 1f; // Radius of damage area
    [SerializeField] private LayerMask playerLayer; // Layer mask for player detection

    [Header("Aura Detection")]
    public float auraDetectionRange = 25f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = true;
        agent.updatePosition = true;
        SetAssassinationState(true);
        SetIsVulnerable(true);
    }

    private void Update()
    {
        // agent.SetDestination(idlePoint.position);
        // if (health <= 0f && currentState != State.Die)
        // {
        //     TransitionTo(State.Die);
        //     // enabled = false;
        //     return;
        // }
        if (agent.enabled == false) return;

        switch (currentState)
        {
            case State.Unaware:
                HandleUnaware();
                break;
            case State.Follow:
                HandleFollow();
                break;
            // case State.AwaitAttack:
            //     HandleAwaitAttack();
            //     break;
            case State.Die:
                HandleDead();
                break;
            case State.Attack:
                // Attack handled via coroutine/animation event
                break;
        }
    }

    #region State Handlers

    private void HandleUnaware()
    {
        // Idle at designated point

        if (Vector3.Distance(transform.position, idlePoint.position) < 1f)
        {
            agent.isStopped = true;
            animator.SetBool(AnimFollow, false);
            animator.SetBool(AnimAttack, false);
            // Debug.Log("Idle at point");
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(idlePoint.position);
            animator.SetBool(AnimFollow, true);
            animator.SetBool(AnimAttack, false);
        }
        

        // Normal line-of-sight detection
        bool normalDetection = Vector3.Distance(transform.position, player.position) <= detectionRange 
                            && IsPlayerInFront();

        // Aura-based detection
        bool auraDetection = false;
        if (player.GetComponent<PlayerAuraManager>().auraActive)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, auraDetectionRange, playerLayer);
            foreach (Collider col in hits)
            {
                if (col.transform == player)
                {
                    auraDetection = true;
                    break;
                }
            }
        }

        if (normalDetection || auraDetection)
        {
            DangerNoti.instance.InvokeNoti();
            TransitionTo(State.Follow);
        }
    }

    private void HandleDead() {
        // if (enabled == false) return;
        // Handle death state
        if (agent.enabled == false) return;
        // animator.CrossFade("Dead", 0.0f);
        GameManager.instance.OnEnemyKilled();
        agent.isStopped = true;
        agent.enabled = false;
        animator.SetBool(AnimAttack, false);
        // animator.CrossFade("Dead", 0.2f);
        StopAllCoroutines();
        // enabled = false;
    }

    private void HandleFollow()
    {
        animator.SetBool(AnimFollow, true);
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        // if (dist <= followDistance)
        // {
        //     TransitionTo(State.AwaitAttack);
        // }
        // else 
        if (dist > detectionRange * 1.2f) // lose interest if too far
        {
            TransitionTo(State.Unaware);
        }
        else if (dist <= attackDistance) {
            TransitionTo(State.Attack);
            StartCoroutine(PerformAttack());
        }
    }

    private void HandleAwaitAttack()
    {
        // State entry setup
        if (!animator.GetBool(AnimOrbiting)) {
            animator.SetBool(AnimFollow, false);
            animator.SetBool(AnimAttack, false);
            animator.SetBool(AnimOrbiting, true);
            
            // Calculate current angle based on position relative to player
            Vector3 relativePos = transform.position - player.position;
            currentOrbitAngle = Mathf.Atan2(relativePos.z, relativePos.x) * Mathf.Rad2Deg;
            
            // Reset attack delay timer
            orbitAttackDelayTimer = 0f;
        }
        
        // Orbit calculation
        currentOrbitAngle += orbitDirection * orbitSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(
            Mathf.Cos(currentOrbitAngle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(currentOrbitAngle * Mathf.Deg2Rad)
        ) * orbitRadius;
        Vector3 orbitPoint = player.position + offset;
        
        // Check if actually orbiting or still moving to orbit position
        float distanceToOrbitPoint = Vector3.Distance(transform.position, orbitPoint);
        if (distanceToOrbitPoint > orbitRadius * 0.3f) {
            // Still moving closer to orbit position
            animator.SetFloat(AnimOrbit, 0);
        } else {
            // Actually orbiting, determine direction based on relative movement
            Vector3 targetDir = orbitPoint - transform.position;
            Vector3 rightDir = transform.right;
            float dotProduct = Vector3.Dot(targetDir.normalized, rightDir.normalized);
            int targetDirection = dotProduct > 0 ? 1 : -1;
            
            // Smooth animation transition
            float smoothedOrbitValue = Mathf.Lerp(animator.GetFloat(AnimOrbit), targetDirection, 2f * Time.deltaTime);
            animator.SetFloat(AnimOrbit, smoothedOrbitValue);
            
            // For orbit calculation (consider changing orbitDirection from int to float for smoother movement)
            orbitDirection = targetDirection;
        }
        
        // Apply navigation - ensure agent is active for orbiting
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.SetDestination(orbitPoint);
        
        // Make enemy face the player while orbiting
        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0; // Keep rotation only in the horizontal plane
        if (dirToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }

        // Update attack delay timer
        orbitAttackDelayTimer += Time.deltaTime;
        
        // Random chance to attack, but only after delay
        if (orbitAttackDelayTimer >= 1.5f && Random.value < attackChancePerSecond * Time.deltaTime)
        {
            TransitionTo(State.Attack);
            StartCoroutine(PerformAttack());
        }

        // If player runs away
        if (Vector3.Distance(transform.position, player.position) > followDistance * 1.2f)
        {
            animator.SetBool(AnimOrbiting, false);
            TransitionTo(State.Follow);
        }
    }

    #endregion

    #region Transitions & Attack
    private IEnumerator PerformAttack()
    {
        // Signal attack preparation
        // animator.SetBool(AnimOrbiting, false);
        // animator.SetFloat(AnimOrbit, 0);
        
        // First phase: approach and position for attack
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distToPlayer > attackDistance) {           
            agent.isStopped = false;
            animator.SetBool(AnimFollow, true);
            
            // Wait until close enough or timeout
            float approachTimer = 0f;
            while (Vector3.Distance(transform.position, player.position) > attackDistance && approachTimer < 2.5f)
            {
                agent.SetDestination(player.position);
                approachTimer += Time.deltaTime;
                yield return null;
            }
        }
        
        // Second phase: telegraph the attack (wind-up)
        agent.isStopped = true;
        animator.SetBool(AnimFollow, false);
        // animator.SetBool(AnimOrbiting, false);
        
        // Track player during wind-up phase
        float windUpTime = Random.Range(0.2f, 0.6f); // Variable wind-up timing
        float windUpTimer = 0f;
        
        while (windUpTimer < windUpTime)
        {
            // Track player position during wind-up for that Elden Ring feel
            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 8f * Time.deltaTime);
            
            windUpTimer += Time.deltaTime;
            yield return null;
        }
        
        // Optional delay after wind-up (like Elden Ring's delayed attacks)
        if (Random.value < 0.4f) {
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        }
        
        // Third phase: commit to attack with reduced tracking
        animator.SetBool(AnimAttack, true);
        animator.applyRootMotion = true;
        
        // Wait for attack animation to reach damage frame
        yield return new WaitForSeconds(0.3f);
        
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null;
        }
        
        // Fourth phase: recovery period where vulnerable
        animator.SetBool(AnimAttack, false);
        
        // Reset and return to orbiting state
        yield return new WaitForSeconds(0.3f);
        animator.applyRootMotion = false;
        agent.updatePosition = true;
        agent.isStopped = false;
        
        // TransitionTo(State.AwaitAttack);
        TransitionTo(State.Follow);
        animator.SetBool(AnimOrbiting, true);
        animator.SetFloat(AnimOrbit, 0);
    }
    
    private void TransitionTo(State newState)
    {
        currentState = newState;
    }

    private bool IsPlayerInFront()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        Vector3 facing = transform.forward; // Use agent's facing direction
        return Vector3.Dot(facing, dirToPlayer) > 0.5f; // 60 degrees
    }

    #endregion

    public void DamagePlayer()
    {
        // Debug.Log("Damage player");
        Collider[] colliders = Physics.OverlapSphere(damageCenter.position, damageRadius, playerLayer);
        if (colliders.Length > 0)
        {
            // Debug.Log("Player in damage radius");
            // Assuming player has a method to take damage
            colliders[0].TryGetComponent(out PlayerHealth playerHealth);
            if (playerHealth != null)
            {
                // Debug.Log("Player health found");
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        AudioManager.Instance.PlaySound(SoundType.Hit_Enemy);
        if (health <= 0f)
        {
            animator.CrossFade("Dead", 0.2f);
            agent.isStopped = true;
            GetComponent<Collider>().enabled = false;
            animator.applyRootMotion = true;
            TransitionTo(State.Die);
        }
        else
        {
            animator.CrossFade("Hit", 0.2f);
            // TransitionTo(State.AwaitAttack);
        }
    }

    public override void Get_Assassinated(Transform backStabPos)
    {
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;
        transform.position = backStabPos.position;
        transform.forward = backStabPos.forward;
        animator.applyRootMotion = true;
        // animator.SetTrigger(AnimBackStab);
        animator.CrossFade("Backstab", 0.2f);
        AudioManager.Instance.PlaySound(SoundType.Hit_Enemy);
        health = 0;
        TransitionTo(State.Die);
        // enabled = false;

        // TODO: Trigger death Event (Counter or something)
    }

    // Optionally, a public method for external “forget player” calls:
    public void ForgetPlayer()
    {
        TransitionTo(State.Unaware);
    }

    public void OnDrawGizmosSelected()
    {
        // Detection range - red wire sphere
        Gizmos.color = new Color(1, 0, 0, 1.0f);
        Gizmos.DrawWireSphere(damageCenter.position, damageRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, auraDetectionRange);
    }
}
