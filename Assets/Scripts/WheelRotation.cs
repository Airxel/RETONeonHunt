using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WheelRotation : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent enemyAgent;
    [SerializeField]
    private float rotationSpeedFactor = 0.1f;
    [SerializeField]
    private float rotationSpeed;

    private void Update()
    {
        rotationSpeed = enemyAgent.speed;

        transform.Rotate(Vector3.down, rotationSpeedFactor * rotationSpeed * Time.deltaTime);
    }
}
