using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField]
    private Transform playerController;
    [SerializeField]
    private Transform playerRobot;
    [SerializeField]
    private bool isPlayerInVision;
    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = playerController.position;
    }

    private void Update()
    {
        PlayerDetectionSystem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == playerRobot)
        {
            isPlayerInVision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == playerRobot)
        {
            isPlayerInVision = false;
        }
    }

    private void PlayerDetectionSystem()
    {
        if (isPlayerInVision)
        {
            Vector3 povDirection = (playerRobot.position - transform.position).normalized;

            Ray ray = new Ray(transform.position + Vector3.up, povDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform == playerRobot)
                {
                    playerController.position = startingPosition;
                }
            }
        }
    }

}
