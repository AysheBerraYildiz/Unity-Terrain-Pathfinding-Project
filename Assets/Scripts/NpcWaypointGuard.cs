
using System.Collections.Generic;
using UnityEngine;

public class NpcWaypointGuard : MonoBehaviour
{
    public enum GuardState
    {
        Patrol,
        Chase,
        Return
    }

    [Header("References")]
    public WaypointGraph graph;
    public Transform player;
    public Waypoint[] patrolRoute;

    [Header("Movement")]
    public float moveSpeed = 30f;
    public float arrivalDistance = 1f;

    [Header("Vision")]
    public float viewDistance = 100f;
    [Range(0f, 360f)]
    public float viewAngle = 120f;
    public float loseSightTime = 3f;
    public LayerMask obstacleMask;

    [Header("Pathfinding")]
    public float replanInterval = 0.5f;

    [Header("Patrol")]
    public bool pingPong = false;

    private int patrolIndex = 0;
    private int patrolDir = 1;

    private GuardState currentState = GuardState.Patrol;

    private List<Waypoint> currentPath = new List<Waypoint>();
    private int pathIndex = 0;

    private float lastSeenTime = -100f;
    private float nextReplanTime = 0f;

    private Vector3 lastSeenPosition;

    private int returnPatrolIndex = 0;

    private void Start()
    {
        if (graph == null || player == null)
        {
            Debug.LogError("NPC: Graph veya Player atanmamis!");
            enabled = false;
            return;
        }

        graph.BuildGraph();

        if (patrolRoute == null || patrolRoute.Length == 0)
        {
            Debug.LogError("NPC: Patrol Route bos!");
            enabled = false;
            return;
        }

        Debug.Log("NPC sistemi baslatildi.");
    }

    private void Update()
    {
        bool canSee = CanSeePlayer();

        if (canSee)
        {
            lastSeenTime = Time.time;
            lastSeenPosition = player.position;

            currentState = GuardState.Chase;
        }

        switch (currentState)
        {
            case GuardState.Patrol:
                Patrol();
                break;

            case GuardState.Chase:
                Chase(canSee);
                break;

            case GuardState.Return:
                ReturnToPatrol();
                break;
        }
    }

    private void Patrol()
    {
        Waypoint target = patrolRoute[patrolIndex];

        if (target == null)
            return;

        MoveTo(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position)
            <= arrivalDistance)
        {
            AdvancePatrol();
        }
    }

    private void AdvancePatrol()
    {
        if (pingPong)
        {
            if (patrolIndex + patrolDir < 0 ||
                patrolIndex + patrolDir >= patrolRoute.Length)
            {
                patrolDir *= -1;
            }

            patrolIndex += patrolDir;
        }
        else
        {
            patrolIndex = (patrolIndex + 1) % patrolRoute.Length;
        }
    }

    private void Chase(bool canSee)
    {
        if (canSee)
        {
            currentPath.Clear();
            MoveTo(player.position);
        }
        else
        {
            if (Time.time - lastSeenTime >= loseSightTime)
            {
                FindReturnWaypoint();
                currentState = GuardState.Return;
                return;
            }

            if (Time.time >= nextReplanTime)
            {
                PlanPath(lastSeenPosition);
                nextReplanTime = Time.time + replanInterval;
            }

            FollowPath();
        }
    }

    private void FindReturnWaypoint()
    {
        float shortestDistance = Mathf.Infinity;

        returnPatrolIndex = 0;

        for (int i = 0; i < patrolRoute.Length; i++)
        {
            float distance = Vector3.Distance(
                transform.position,
                patrolRoute[i].transform.position
            );

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                returnPatrolIndex = i;
            }
        }

        PlanPath(patrolRoute[returnPatrolIndex].transform.position);
    }

    private void ReturnToPatrol()
    {
        FollowPath();

        if (currentPath.Count == 0 ||
            pathIndex >= currentPath.Count)
        {
            patrolIndex = returnPatrolIndex;
            currentState = GuardState.Patrol;
        }
    }

    private void PlanPath(Vector3 destination)
    {
        Waypoint start = graph.GetNearestWaypoint(transform.position);
        Waypoint goal = graph.GetNearestWaypoint(destination);

        if (start == null || goal == null)
            return;

        int expandedCount;

        currentPath = GraphAStar.FindPath(
            start,
            goal,
            true,
            out expandedCount
        );

        pathIndex = 0;

        Debug.Log("NPC Path Expanded: " + expandedCount);
    }

    private void FollowPath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        if (pathIndex >= currentPath.Count)
            return;

        Waypoint target = currentPath[pathIndex];

        if (target == null)
            return;

        MoveTo(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position)
            <= arrivalDistance)
        {
            pathIndex++;
        }
    }

    private void MoveTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                5f * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );
    }

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 eyePosition =
            transform.position + Vector3.up * 2.5f;

        Vector3 targetPosition =
            player.position + Vector3.up * 2.5f;

        Vector3 direction = targetPosition - eyePosition;

        float distance = direction.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > viewAngle / 2f)
            return false;

        bool blocked = Physics.Linecast(
            eyePosition,
            targetPosition,
            obstacleMask
        );

        return !blocked;
    }
}

