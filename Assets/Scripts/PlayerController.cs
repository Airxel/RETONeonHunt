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
    private GameObject playerBody;
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
    private float frontalTiltSmoothSpeed = 5f;
    [SerializeField]
    private float lateralTiltSmoothSpeed = 0.1f;
    [SerializeField]
    private float tiltResetSpeed = 5f;
    private float lateralTiltVelocity = 0.0f;
    [SerializeField]
    private float rotationSmoothSpeed = 0.1f;

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

    private Animator animator;

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
        animator = playerWheel.GetComponent<Animator>();
    }

    private void Update()
    {
        playerWheel.transform.position = ballRb.transform.position;
        cameraTarget.transform.position = new Vector3(playerBody.transform.position.x, cameraTarget.transform.position.y, playerBody.transform.position.z);
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerShooting();
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
            // Convertir la dirección a grados en relación a la cámara
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
        }

        Vector3 playerVelocity = ballRb.velocity;

        float velocityMagnitude = new Vector2(playerVelocity.x, playerVelocity.z).magnitude; // Magnitud horizontal

        // Suavizar la rotación del cuerpo
        bodyRotation = Mathf.LerpAngle(bodyRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);

        // Inclinación solo si hay velocidad
        if (velocityMagnitude > 0.1f)
        {
            // Inclinación frontal siempre hacia adelante
            float frontalTiltAmount = Mathf.Clamp(velocityMagnitude * tiltFactor, 0.0f, tiltLimit);

            frontalTilt = Mathf.Lerp(frontalTilt, frontalTiltAmount, Time.deltaTime * frontalTiltSmoothSpeed * 10f);

            // Suavizar inclinación lateral evitando saltos bruscos
            float rotationDifference = Mathf.DeltaAngle(bodyRotation, targetRotation) * 0.05f;

            float lateralTiltAmount = Mathf.Clamp(rotationDifference, -tiltLimit, tiltLimit); // Factor más bajo para suavizar

            // Evitar cambios bruscos con SmoothDamp
            lateralTilt = Mathf.SmoothDamp(lateralTilt, lateralTiltAmount, ref lateralTiltVelocity, lateralTiltSmoothSpeed);
        }
        else
        {
            // Si no se mueve, reducir inclinaciones gradualmente
            frontalTilt = Mathf.Lerp(frontalTilt, 0.0f, Time.deltaTime * tiltResetSpeed * 100f);
            lateralTilt = Mathf.Lerp(lateralTilt, 0.0f, Time.deltaTime * tiltResetSpeed * 100f);
        }

        //  Aplicar la rotación del cuerpo
        playerBody.transform.rotation = Quaternion.Euler(frontalTilt, bodyRotation, -lateralTilt);
        playerWheel.transform.rotation = Quaternion.Euler(frontalTilt * 0.5f, bodyRotation, -lateralTilt * 0.5f);

        // Mover la bola en la dirección correcta
        Vector3 newPlayerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward * inputActions.playerMove.magnitude;
        ballRb.AddForce(newPlayerMovement * playerSpeed);
    }

    private void PlayerShooting()
    {
        if (inputActions.playerShoot)
        {
            Debug.Log("Shooting");

            animator.SetTrigger("Shoot");

            inputActions.playerShoot = false;
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
