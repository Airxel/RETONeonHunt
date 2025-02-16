using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player")]
    private InputActions inputActions;
    private CharacterController characterController;
    [SerializeField]
    private GameObject playerBody;
    [SerializeField]
    private float playerSpeed = 10f;
    [SerializeField]
    private float rotationSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Camera")]
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject cameraTarget;
    [SerializeField]
    private float topCameraLimit = 70f;
    [SerializeField]
    private float bottomCameraLimit = -30f;
    [SerializeField]
    private float horizontalCameraLimit = 90f;
    [SerializeField]
    private float cameraRotationSpeed = 10f;
    private float cameraTargetYaw;
    private float cameraTargetPitch;
    private float targetRotation;

    private void Awake()
    {
        inputActions = GetComponent<InputActions>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerNewMovement();
    }

    private void PlayerNewMovement()
    {
        Vector3 playerDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (inputActions.playerMove.magnitude >= 0.1f)
        {
            //Cambio de dirección del jugador según el input
            float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float targetAngleSmooth = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, targetAngleSmooth, 0.0f);

            //Movimiento en la dirección en la que esté mirando el jugador
            Vector3 newPlayerDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

            characterController.Move(newPlayerDirection * playerSpeed * Time.deltaTime);
        }
    }

    private void CameraRotation()
    {
        if (inputActions.playerLook.sqrMagnitude > 0.01f)
        {
            cameraTargetYaw += inputActions.playerLook.x * cameraRotationSpeed * Time.deltaTime;
            cameraTargetPitch += inputActions.playerLook.y * cameraRotationSpeed * Time.deltaTime;
        }

        cameraTargetYaw = Mathf.Clamp(cameraTargetYaw, -horizontalCameraLimit, horizontalCameraLimit);
        cameraTargetPitch = Mathf.Clamp(cameraTargetPitch, bottomCameraLimit, topCameraLimit);

        cameraTarget.transform.rotation = Quaternion.Euler(cameraTargetPitch, cameraTargetYaw, 0.0f);
    }
}
