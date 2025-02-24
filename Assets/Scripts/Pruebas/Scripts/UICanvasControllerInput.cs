using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public InputActions inputActions;
        public PlayerInput playerInput;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            inputActions.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            Debug.Log($"Joystick Virtual Look Input: {virtualLookDirection}");
            inputActions.LookInput(virtualLookDirection);
        }

        public void VirtualShootInput(bool virtualShootState)
        {
            inputActions.ShootInput(virtualShootState);
        }
    }

}
