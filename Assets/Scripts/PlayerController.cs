using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody ballRb;
    private float playerMovementX;
    private float playerMovementY;

    [SerializeField]
    public float playerSpeed;

    [SerializeField]
    public float playerTilt;
    private float tiltAngle;

    private void FixedUpdate()
    {
        Vector3 playerMovement = new Vector3(playerMovementX, 0.0f, playerMovementY);

        ballRb.AddForce(playerMovement * playerSpeed);

        Vector3 playerVelocity = ballRb.velocity;

        if (playerVelocity.magnitude > 0.1f)
        {
            tiltAngle = Mathf.Clamp(playerVelocity.x * playerTilt, -playerTilt, playerTilt);
        }

        ballRb.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, -tiltAngle);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 playerDirection = movementValue.Get<Vector2>();

        playerMovementX = playerDirection.x;
        playerMovementY = playerDirection.y;
    }
}