using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WheelRotation : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent enemyAgent;
    [SerializeField]
    private float rotationSpeedFactor;
    [SerializeField]
    private float rotationSpeed;

    private void Start()
    {
        rotationSpeed = enemyAgent.speed;
        rotationSpeedFactor = rotationSpeed * 10f;
    }

    private void Update()
    {
        transform.Rotate(Vector3.down, rotationSpeedFactor * rotationSpeed * Time.deltaTime);
    }
}
