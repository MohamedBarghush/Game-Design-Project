using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfManager : MonoBehaviour
{
    public static WolfManager instance;

    void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class StopPoint
    {
        public int waypointIndex;
        public bool useBusyAnimation = true;
    }

    [Header("Navigation")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float stoppingDistance = 1f;
    [SerializeField] List<StopPoint> stopPoints = new List<StopPoint>();

    [Header("Detection")]
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float checkInterval = 0.5f;

    [Header("Animation")]
    [SerializeField] string busyParam = "busy";

    Animator animator;
    NavMeshAgent agent;
    int currentWaypoint;
    float checkTimer;
    bool isWaitingAtStopPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.stoppingDistance = stoppingDistance;
        MoveToWaypoint(0);
    }

    void Update()
    {
        if (isWaitingAtStopPoint) return;

        checkTimer += Time.deltaTime;
        animator.SetBool("move", agent.velocity.magnitude > 0.1f);

        if (ReachedDestination() && checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            CheckWaypointConditions();
        }
    }

    void CheckWaypointConditions()
    {
        if (IsStopPoint(currentWaypoint))
        {
            HandleStopPoint();
            return;
        }

        if (PlayerInDetectionRadius())
            ProgressToNextWaypoint();
    }

    void HandleStopPoint()
    {
        isWaitingAtStopPoint = true;
        var stopPoint = GetStopPoint(currentWaypoint);
        if (stopPoint.useBusyAnimation)
            animator.SetBool(busyParam, true);
    }

    public void OnVillageCleared()
    {
        if (!isWaitingAtStopPoint) return;

        var stopPoint = GetStopPoint(currentWaypoint);
        if (stopPoint.useBusyAnimation)
            animator.SetBool(busyParam, false);

        isWaitingAtStopPoint = false;
        ProgressToNextWaypoint();
    }

    void ProgressToNextWaypoint()
    {
        currentWaypoint = Mathf.Clamp(currentWaypoint + 1, 0, waypoints.Length - 1);
        MoveToWaypoint(currentWaypoint);
    }

    public void MoveToWaypoint(int index)
    {
        if (index >= waypoints.Length) return;
        agent.SetDestination(waypoints[index].position);
    }

    bool ReachedDestination()
    {
        return !agent.pathPending 
               && agent.remainingDistance <= agent.stoppingDistance
               && agent.velocity.sqrMagnitude == 0f;
    }

    bool PlayerInDetectionRadius()
    {
        if (currentWaypoint >= waypoints.Length) return false;
        return Physics.CheckSphere(
            waypoints[currentWaypoint].position,
            detectionRadius,
            playerLayer
        );
    }

    bool IsStopPoint(int index) => GetStopPoint(index) != null;
    StopPoint GetStopPoint(int index) => 
        stopPoints.Find(sp => sp.waypointIndex == index);

    void OnDrawGizmosSelected()
    {
        if (waypoints == null || currentWaypoint >= waypoints.Length) return;
        
        Gizmos.color = IsStopPoint(currentWaypoint) ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(
            waypoints[currentWaypoint].position,
            detectionRadius
        );
    }
}