using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider detectionCollider;
    [SerializeField]
    private Transform playerRobot;
    [SerializeField]
    private GameObject playerParent;
    [SerializeField]
    private float detectionColliderRange = 20f;
    [SerializeField]
    private bool isPlayerInVision;

    private void Update()
    {
        DetectionColliderSize();
        PlayerDetectionSystem();
    }

    /// <summary>
    /// Función para comprobar si el jugador está en el rango de detección
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == playerRobot)
        {
            isPlayerInVision = true;
        }
    }

    /// <summary>
    /// Función para comprobar si el jugador sale del rango de detección
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == playerRobot)
        {
            isPlayerInVision = false;
        }
    }

    /// <summary>
    /// Función para detectar al jugador, siempre y cuando no haya un obstáculo en medio
    /// </summary>
    private void PlayerDetectionSystem()
    {
        if (isPlayerInVision)
        {
            Vector3 povDirection = playerRobot.position - transform.position + Vector3.up;

            Ray ray = new Ray(transform.position, povDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform == playerRobot)
                {
                    //UnityEngine.Cursor.lockState = CursorLockMode.None;
                    playerParent.SetActive(false);
                    UIManager.instance.isGameRunning = false; 
                    UIManager.instance.defeatScreen.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Función para controlar el tamaño del área de detección desde el inspector
    /// </summary>
    private void DetectionColliderSize()
    {
        detectionCollider.center = new Vector3(detectionColliderRange / 2, detectionCollider.center.y, detectionCollider.center.z);
        detectionCollider.height = detectionColliderRange;
    }
}
