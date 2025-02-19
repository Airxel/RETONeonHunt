using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (other.CompareTag("Player"))
        {
            isPlayerInVision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
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

            if (Physics.Raycast(ray, out hit, ~LayerMask.GetMask("Ignore Raycast")))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerController.position = startingPosition;

                    Debug.DrawRay(transform.position + Vector3.up, povDirection * 100f, Color.yellow, 3f);
                }
                else
                {
                    Debug.DrawRay(transform.position + Vector3.up, povDirection * 100f, Color.blue, 3f);
                }
                
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            }
        }
    }

}
