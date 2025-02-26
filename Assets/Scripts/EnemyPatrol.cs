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

    /// <summary>
    /// Se actualiza el primer punto de patrulla al primer punto en la lista y se añade un enemigo al total
    /// </summary>
    private void Start()
    {
        navMeshAgent.SetDestination(patrolPoints[0].position);
        UIManager.instance.EnemiesManager(1);
    }

    private void Update()
    {
        EnemyPatrolPathing();
    }

    /// <summary>
    /// Función que controla a que punto de patrulla sigue el enemigo, una vez llegue al destino
    /// </summary>
    private void EnemyPatrolPathing()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;

            navMeshAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }
}
