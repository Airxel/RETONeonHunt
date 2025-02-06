using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody bodyRb;
    private Rigidbody playerRb;
    private float playerMovementX;
    private float playerMovementY;

    [SerializeField]
    public float playerSpeed;

    [SerializeField]
    public float playerTilt;
    private float tiltAngle;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 playerMovement = new Vector3(playerMovementX, 0.0f, playerMovementY);

        playerRb.AddForce(playerMovement * playerSpeed);

        Vector3 playerVelocity = playerRb.velocity;

        if (playerVelocity.magnitude > 0.1f)
        {
            tiltAngle = Mathf.Clamp(playerVelocity.x * playerTilt, -playerTilt, playerTilt);
        }

        playerRb.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, -tiltAngle);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 playerDirection = movementValue.Get<Vector2>();

        playerMovementX = playerDirection.x;
        playerMovementY = playerDirection.y;
    }
}
