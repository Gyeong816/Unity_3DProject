using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : MonoBehaviour
{
    private NavMeshAgent nav;
    [SerializeField] private Transform[] patrolPoints;
    private int currentIndex = 0;

    [SerializeField] private float patrolWaitTime = 1.0f;
    private float waitTimer;

    [SerializeField] private float arrivalThreshold = 0.5f;

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;

    private bool isChasing = false;
    private float lostSightTimer = 0f;
    [SerializeField] private float lostSightDuration = 5f;

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
            nav.SetDestination(patrolPoints[0].position);
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            lostSightTimer = 0f;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > 2.5f)
            {
                nav.stoppingDistance = 2.5f;
                nav.SetDestination(player.position);
            }
            else
            {
                nav.ResetPath();
            }

            isChasing = true;
        }
        else
        {
            if (isChasing)
            {
                lostSightTimer += Time.deltaTime;

                if (lostSightTimer >= lostSightDuration)
                {
                    isChasing = false;
                    nav.stoppingDistance = 0f;
                    Patrol();
                }
            }
            else
            {
                Patrol();
            }
        }

        DrawView();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0 || isChasing) return;

        if (!nav.pathPending && nav.remainingDistance <= arrivalThreshold)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                nav.SetDestination(patrolPoints[currentIndex].position);
                waitTimer = 0f;
            }
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange) return false;

        float angleBetween = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleBetween > viewAngle * 0.5f) return false;

        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, detectionRange, ~obstacleMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private void DrawView()
    {
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Debug.DrawRay(transform.position, leftDir * detectionRange, Color.yellow);
        Debug.DrawRay(transform.position, rightDir * detectionRange, Color.yellow);
        Debug.DrawRay(transform.position, transform.forward * detectionRange, Color.red);
    }
}
