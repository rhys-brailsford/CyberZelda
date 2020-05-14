using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;

    // This shouldn't be called playerLayer as this is a layerMask and therefore actually uses multiple layers
    // and expects multiple layers to be selected in the inspector
    public LayerMask sightLayers;
    public float staticSightRange = 10;
    public float followSightRange = 15;
    private float curSightRange;
    public bool debug = false;

    public EnemyState enemyState = EnemyState.Passive;
    private EnemyMovementState moveState = EnemyMovementState.Idle;

    // Patrolling and leashing variables
    public List<GameObject> patrolObjs = new List<GameObject>();
    private List<Vector3> patrolPositions = new List<Vector3>();
    private bool patrols = false;
    public float minLeashDistance = 15;
    public float maxLeashDistance = 30;
    private Vector3 curPatrolDest;
    private int curPatrolDestIndex;

    // Knockback variables
    public float knockbackDuration = 1;
    public float knockbackStrength = 10;
    private bool knockedBack = false;

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Color myColor = Color.blue;
            myColor.a = 0.5f;
            Gizmos.color = myColor;
            Gizmos.DrawWireSphere(gameObject.transform.position, curSightRange);
        }
    }

    void Awake()
    {
        // Extract positions of patrol gameobjects for patrolling positions
        foreach (GameObject patrolObj in patrolObjs)
        {
            patrolPositions.Add(patrolObj.transform.position);
        }

        // Ensure patrol positions has at least one element
        // By default, use enemy's original position
        if (patrolPositions.Count == 0)
        {
            patrolPositions.Add(gameObject.transform.position);
        }
        else if (patrolPositions.Count == 1 && patrolPositions[0] == Vector3.zero)
        {
            patrolPositions[0] = gameObject.transform.position;
        }

        if (patrolPositions.Count > 1)
        {
            patrols = true;
            moveState = EnemyMovementState.Patrolling;
            curPatrolDestIndex = 0;
            curPatrolDest = patrolPositions[curPatrolDestIndex];
            agent.SetDestination(curPatrolDest);
        }
    }

    void Update()
    {
        // Only look for player if aggro'd, not knockedBack, and not leashing
        if (enemyState == EnemyState.Aggro && !knockedBack && moveState != EnemyMovementState.Leashing)
        {
            if (CanSeePlayer())
            {
                Vector3 playerPos = GameManager.GM.GetPlayer().transform.position;
                agent.SetDestination(playerPos);
                moveState = EnemyMovementState.Following;
            }
        }

        if (moveState == EnemyMovementState.Following)
        {
            UpdatePatrolDestination();
            float distFromLeash = Vector3.Distance(gameObject.transform.position, curPatrolDest);

            if (distFromLeash > maxLeashDistance)
            {
                moveState = EnemyMovementState.Leashing;
                agent.SetDestination(curPatrolDest);
            }
        }
        else if (moveState == EnemyMovementState.Leashing)
        {
            UpdatePatrolDestination();
            float distFromLeash = Vector3.Distance(gameObject.transform.position, curPatrolDest);

            if (distFromLeash < minLeashDistance)
            {
                moveState = EnemyMovementState.Resetting;
            }
        }
        else if (moveState == EnemyMovementState.Resetting)
        {
            if (ReachedDestination())
            {
                if (patrols)
                {
                    moveState = EnemyMovementState.Patrolling;
                    UpdatePatrolDestination();
                    agent.SetDestination(curPatrolDest);
                }
                else
                {
                    moveState = EnemyMovementState.Idle;
                }
            }
        }

        // Not part of same else if chain because state can change to Patrolling and we want it to start
        // patrolling immediately.
        if (moveState == EnemyMovementState.Patrolling)
        {
            if (ReachedDestination())
            {
                curPatrolDestIndex = (curPatrolDestIndex + 1) % patrolPositions.Count;
                curPatrolDest = patrolPositions[curPatrolDestIndex];
                agent.SetDestination(curPatrolDest);
            }
        }
    }

    // Returns true if agent has reached its destination, false otherwise.
    private bool ReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Updates EnemyMovement.curPatrolDest and EnemyMovement.curPatrolDestIndex based on closest patrolPositions
    // relative to enemy position.
    private void UpdatePatrolDestination()
    {
        float shortestDist = float.PositiveInfinity;
        for (int i=0; i<patrolPositions.Count; i++)
        {
            Vector3 curPatrol = patrolPositions[i];
            float curDist = Vector3.Distance(gameObject.transform.position, curPatrol);

            if (curDist < shortestDist)
            {
                shortestDist = curDist;
                curPatrolDest = curPatrol;
                curPatrolDestIndex = i;
            }
        }
    }

    private bool CanSeePlayer()
    {
        curSightRange = moveState == EnemyMovementState.Following ? followSightRange : staticSightRange;

        Vector3 playerPos = GameManager.GM.GetPlayer().transform.position;

        Ray ray = new Ray(gameObject.transform.position, playerPos - gameObject.transform.position);
        RaycastHit hitInfo;
        bool result = Physics.Raycast(ray, out hitInfo, curSightRange, sightLayers);

        if (debug)
        {
            Debug.DrawLine(gameObject.transform.position, agent.destination, Color.red);
        }

        if (result)
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void Aggro()
    {
        enemyState = EnemyState.Aggro;
        moveState = EnemyMovementState.Following;
    }

    public void Knockback(Vector3 direction)
    {
        StartCoroutine("KnockbackCoroutine", direction);
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction)
    {
        float duration = 0;
        agent.isStopped = true;
        knockedBack = true;

        while (duration < knockbackDuration)
        {
            gameObject.transform.position = gameObject.transform.position + (direction * knockbackStrength * Time.deltaTime);
            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        knockedBack = false;
        agent.isStopped = false;
    }
}
