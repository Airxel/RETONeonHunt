using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    private Rigidbody ballRb;
    private InputActions inputActions;
    [SerializeField]
    private GameObject playerRobot;
    [SerializeField]
    private GameObject playerWheel;
    [SerializeField]
    private float playerSpeed = 5f;
    private float lateralTilt;
    private float frontalTilt;
    private float bodyRotation;
    [SerializeField]
    private float tiltFactor = 5f;
    [SerializeField]
    private float tiltLimit = 25f;
    [SerializeField]
    private float tiltResetSpeed = 5f;

    [SerializeField]
    private float rotationSmoothSpeed = 0.1f; // Velocidad de suavizado de la rotaci�n
    private float rotationVelocity; // Almacena la velocidad del suavizado

    [Header("Cinemachine")]
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

    private float lateralTiltVelocity = 0.0f; // Para usar con SmoothDamp

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
    }

    private void Update()
    {
        playerWheel.transform.position = ballRb.transform.position;
        playerRobot.transform.position = new Vector3(playerWheel.transform.position.x, 1f, playerWheel.transform.position.z);
    }

    private void FixedUpdate()
    {
        PlayerMovement();  
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void PlayerMovement()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (inputActions.playerMove.magnitude > 0.01f)
        {
            // Convertir la direcci�n a grados en relaci�n a la c�mara
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
        }

        Vector3 playerVelocity = ballRb.velocity;

        float velocityMagnitude = new Vector2(playerVelocity.x, playerVelocity.z).magnitude; // Magnitud horizontal

        // Suavizar la rotaci�n del cuerpo
        bodyRotation = Mathf.LerpAngle(bodyRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

        // Inclinaci�n solo si hay velocidad
        if (velocityMagnitude > 0.1f)
        {
            // Inclinaci�n frontal siempre hacia adelante
            float forwardTiltTarget = Mathf.Clamp(velocityMagnitude * tiltFactor, 0.0f, tiltLimit);

            frontalTilt = Mathf.Lerp(frontalTilt, forwardTiltTarget, Time.deltaTime * tiltResetSpeed * 10f);

            // Suavizar inclinaci�n lateral evitando saltos bruscos
            float rotationDifference = Mathf.DeltaAngle(bodyRotation, targetRotation);

            float lateralTiltTarget = Mathf.Clamp(rotationDifference * 0.05f, -tiltLimit, tiltLimit); // Factor m�s bajo para suavizar

            // Evitar cambios bruscos con SmoothDamp
            lateralTilt = Mathf.SmoothDamp(lateralTilt, lateralTiltTarget, ref lateralTiltVelocity, 0.1f);
        }
        else
        {
            // Si no se mueve, reducir inclinaciones gradualmente
            frontalTilt = Mathf.Lerp(frontalTilt, 0.0f, Time.deltaTime * tiltResetSpeed * 100f);
            lateralTilt = Mathf.SmoothDamp(lateralTilt, 0.0f, ref lateralTiltVelocity, 0.2f);
        }

        //  Aplicar la rotaci�n del cuerpo
        playerRobot.transform.rotation = Quaternion.Euler(frontalTilt, bodyRotation, -lateralTilt);
        playerWheel.transform.rotation = Quaternion.Euler(frontalTilt * 0.5f, bodyRotation, -lateralTilt * 0.5f);

        // Mover la bola en la direcci�n correcta
        Vector3 newPlayerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward * inputActions.playerMove.magnitude;
        ballRb.AddForce(newPlayerMovement * playerSpeed);
    }

    private void PlayerMovement2()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (inputActions.playerMove.magnitude > 0.01f)
        {
            // Se convierte la direcci�n a grados, en direcci�n a la c�mara
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
        }

        Vector3 playerVelocity = ballRb.velocity;

        if (playerVelocity.magnitude > 0.1f)
        {
            lateralTilt = Mathf.Clamp(playerVelocity.x * tiltFactor, -tiltLimit, tiltLimit);
            frontalTilt = Mathf.Clamp(playerVelocity.z * tiltFactor, -tiltLimit, tiltLimit);
            bodyRotation = Mathf.LerpAngle(bodyRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

            playerRobot.transform.rotation = Quaternion.Euler(frontalTilt, bodyRotation, -lateralTilt);
            playerWheel.transform.rotation = Quaternion.Euler(frontalTilt * 0.5f, bodyRotation, -lateralTilt * 0.5f);
        }
        else
        {
            lateralTilt = Mathf.Lerp(lateralTilt, 0.0f, Time.deltaTime * tiltResetSpeed * 100f);
            frontalTilt = Mathf.Lerp(frontalTilt, 0.0f, Time.deltaTime * tiltResetSpeed * 100f);
            bodyRotation = Mathf.LerpAngle(bodyRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

            playerRobot.transform.rotation = Quaternion.Euler(frontalTilt, bodyRotation, lateralTilt);
            playerWheel.transform.rotation = Quaternion.Euler(frontalTilt * 0.5f, bodyRotation, -lateralTilt * 0.5f);
        }

        Vector3 newPlayerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward * inputActions.playerMove.magnitude;

        ballRb.AddForce(newPlayerMovement * playerSpeed);
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
