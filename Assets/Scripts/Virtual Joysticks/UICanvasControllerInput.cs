using UnityEngine;
using UnityEngine.InputSystem;

public class UICanvasControllerInput : MonoBehaviour
{

    [Header("Output")]
    public InputActions inputActions;

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        inputActions.MoveInput(virtualMoveDirection);
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        inputActions.LookInput(virtualLookDirection);
    }

    public void VirtualShootInput(bool virtualShootState)
    {
        inputActions.ShootInput(virtualShootState);
    }
}
