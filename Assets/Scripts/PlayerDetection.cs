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
    private GameObject playerParent;
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
        detectionCollider.center = new Vector3(detectionColliderRange / 2, detectionCollider.center.y, detectionCollider.center.z);
        detectionCollider.height = detectionColliderRange;

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
                    //UnityEngine.Cursor.lockState = CursorLockMode.None;
                    playerParent.SetActive(false);
                    UIManager.instance.isGameRunning = false; 
                    UIManager.instance.defeatScreen.SetActive(true);
                    Debug.Log("DEAD");
                }
            }
        }
    }
}
