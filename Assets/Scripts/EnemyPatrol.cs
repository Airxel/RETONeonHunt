using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;
    [SerializeField]
    private Transform[] patrolPoints;
    private int currentPatrolPointIndex;

    private void Start()
    {
        navMeshAgent.SetDestination(patrolPoints[0].position);
    }

    private void Update()
    {
        EnemyPatrolPathing();
    }

    private void EnemyPatrolPathing()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;

            navMeshAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
}
