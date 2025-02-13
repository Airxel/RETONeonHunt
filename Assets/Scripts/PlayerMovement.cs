using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputActions inputActions;
    private Rigidbody ballRb;
    private GameObject mainCamera;

    [Header("Player")]
    [SerializeField]
    private float playerSpeed = 5f;
    [SerializeField]
    private Vector3 playerVelocity;
    [SerializeField]
    private float accelerationFactor = 10f;
    [SerializeField]
    private float decelerationFactor = 10f;
    private float frontalTilt = 0.0f;
    [SerializeField]
    private float frontalTiltAmount = 25f;
    private float lateralTilt = 0.0f;
    [SerializeField]
    private float lateralTiltAmount = 10f;
    [SerializeField]
    private float lateralTiltResetSpeed = 5f;
    [SerializeField]
    private float tiltSmooth = 5.0f;
    private float previousRotation = 0.0f;
    private float rotationVelocity;
    [SerializeField]
    private float rotationSmooth = 5f;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
        previousRotation = transform.eulerAngles.y;
    }

    void FixedUpdate()
    {
        PlayerMove();
    }
    private void PlayerMove()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (playerMovement.magnitude > 0.1f)
        {
            playerVelocity = Vector3.Lerp(playerVelocity, playerMovement * playerSpeed, Time.deltaTime * accelerationFactor);
        }
        else
        {
            playerVelocity = Vector3.Lerp(playerVelocity, Vector3.zero, Time.deltaTime * decelerationFactor);
        }

        ballRb.velocity = new Vector3(playerVelocity.x, ballRb.velocity.y, playerVelocity.z);

        // **Detectar si el personaje está girando**
        float targetRotation = transform.eulerAngles.y;

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la dirección a grados, en dirección a la cámara
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se calcula la inclinación al moverse
            frontalTilt = inputActions.playerMove.magnitude * frontalTiltAmount;
            lateralTilt = inputActions.playerMove.magnitude * lateralTiltAmount;

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation)) > 0.01f)
            {
                lateralTilt = inputActions.playerMove.magnitude * lateralTiltAmount; // Inclinación lateral según el movimiento
            }
            else
            {
                lateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, 0.0f, Time.deltaTime * tiltSmooth);
            }

            float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, frontalTilt, Time.deltaTime * tiltSmooth);
            float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

            // Se aplica la inclinación y rotación
            transform.rotation = Quaternion.Euler(smoothedFrontalTilt, playerRotation, smoothedLateralTilt);
        }
        else
        {
            // Si no hay movimiento, se hace una interpolación para devolver las inclinaciones a 0
            float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, 0.0f, Time.deltaTime * tiltSmooth);
            float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, 0.0f, Time.deltaTime * tiltSmooth);

            // Aplicar la rotación sin inclinación
            transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, smoothedLateralTilt);
        }
        // Rotar hacia la dirección de movimiento
        if (playerMovement != Vector3.zero)
        {
            Quaternion currentRotation = Quaternion.LookRotation(playerMovement);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, Time.deltaTime * rotationSmooth);
        }
    }
}
