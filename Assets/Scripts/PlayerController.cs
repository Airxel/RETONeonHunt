using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private float playerMovementX;
    private float playerMovementY;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 playerMovement = new Vector3(playerMovementX, 0.0f, playerMovementY);

        playerRb.AddForce(playerMovement);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 playerDirection = movementValue.Get<Vector2>();

        playerMovementX = playerDirection.x;
        playerMovementY = playerDirection.y;
    }
}
