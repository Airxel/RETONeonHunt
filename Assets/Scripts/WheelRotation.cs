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
    [SerializeField]
    private GameObject enemyWheelTrail;

    /// <summary>
    /// La velocidad de giro es en función de la velocidad del enemigo
    /// </summary>
    private void Start()
    {
        rotationSpeed = enemyAgent.speed;
        rotationSpeedFactor = rotationSpeed * 10f;
    }

    /// <summary>
    /// Se rota la rueda y el objeto que crea el efecto de la estela
    /// </summary>
    private void Update()
    {
        transform.Rotate(Vector3.down, rotationSpeedFactor * rotationSpeed * Time.deltaTime);
        enemyWheelTrail.transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);
    }
}
