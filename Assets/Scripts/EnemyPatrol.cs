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
    [SerializeField]
    private int currentPatrolPointIndex;

    private void Start()
    {
        navMeshAgent.SetDestination(patrolPoints[0].position);
        UIManager.instance.EnemiesManager(1);
    }

    private void Update()
    {
        EnemyPatrolPathing();
    }

    private void EnemyPatrolPathing()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;

            navMeshAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
}
