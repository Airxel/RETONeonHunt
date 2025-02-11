using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActions : MonoBehaviour
{
    public Vector2 playerMove;
    public Vector2 playerLook;
    public bool playerShoot;

    public bool analogMovement;

    public void OnMove(InputValue movementValue)
    {
        MoveInput(movementValue.Get<Vector2>());
    }

    public void OnLook(InputValue lookValue)
    {
        LookInput(lookValue.Get<Vector2>());
    }

    public void OnShoot(InputValue shootValue)
    {
        ShootInput(shootValue.isPressed);
    }

    private void OnDisable()
    {
        playerMove = Vector2.zero; // Resetea el movimiento al desactivar el script
    }

    private void Update()
    {
        Debug.Log("PlayerMove: " + playerMove); // Verifica en la consola si sigue moviéndose solo
    }

    public void MoveInput(Vector2 newMovementValue)
    {
        playerMove = newMovementValue;
    }

    public void LookInput(Vector2 newLookValue)
    {
        playerLook = newLookValue;
    }

    public void ShootInput(bool newShootState)
    {
        playerShoot = newShootState;
    }
}
