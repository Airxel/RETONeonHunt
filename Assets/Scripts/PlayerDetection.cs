using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDetection : MonoBehaviour
{
    private CapsuleCollider detectionCollider;
    [SerializeField]
    private Transform playerController;
    [SerializeField]
    private Transform playerRobot;
    [SerializeField]
    private float detectionColliderRange = 20f;
    [SerializeField]
    private bool isPlayerInVision;
    private Vector3 startingPosition;

    private void Awake()
    {
        detectionCollider = GetComponent<CapsuleCollider>();
    }

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
            Vector3 povDirection = playerRobot.position - transform.position + Vector3.up;

            Ray ray = new Ray(transform.position, povDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform == playerRobot)
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
