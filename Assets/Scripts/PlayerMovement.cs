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
    [SerializeField]
    private GameObject playerRobot;

    [Header("Player")]
    [SerializeField]
    private float playerSpeed = 5f;
    [SerializeField]
    private Vector3 playerVelocity;
    [SerializeField]
    private float accelerationFactor = 10f;
    [SerializeField]
    private float decelerationFactor = 10f;
    [SerializeField]
    private float frontalTilt = 0.0f;
    [SerializeField]
    private float frontalTiltAmount = 15f;
    [SerializeField]
    private float lateralTilt = 0.0f;
    [SerializeField]
    private float lateralTiltAmount = 10f;
    [SerializeField]
    private float tiltSmooth = 5.0f;
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
    }

    void FixedUpdate()
    {
        PlayerMove2();
    }

    private void PlayerMove3()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (playerMovement.sqrMagnitude > 0.01f)
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, playerMovement * playerSpeed, Time.deltaTime * accelerationFactor);
        }
        else
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, Vector3.zero, Time.deltaTime * decelerationFactor);
        }

        ballRb.velocity = new Vector3(playerVelocity.x, ballRb.velocity.y, playerVelocity.z);

        Vector3 newPlayerVelocity = ballRb.velocity;

        // Aplicar inclinación basada en la velocidad
        if (newPlayerVelocity.magnitude > 0.1f) // Evita inclinaciones cuando está quieto
        {
            float tiltAngle = Mathf.Clamp(playerVelocity.x * lateralTiltAmount, -lateralTilt, lateralTiltAmount);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, -tiltAngle);
        }
        else
        {
            // Resetear inclinación si no hay movimiento
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }

    private void PlayerMove4()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (playerMovement.magnitude > 0.1f)
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, playerMovement * playerSpeed, Time.deltaTime * accelerationFactor);
        }
        else
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, Vector3.zero, Time.deltaTime * decelerationFactor);
        }

        ballRb.velocity = new Vector3(playerVelocity.x, ballRb.velocity.y, playerVelocity.z);

        // **Detectar si el personaje está girando**
        float targetRotation = transform.eulerAngles.y;

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la dirección a grados, en dirección a la cámara
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg; //+ mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se calcula la inclinación al moverse
            frontalTilt = Mathf.Clamp(playerVelocity.z * frontalTiltAmount, -frontalTiltAmount, frontalTiltAmount);
            lateralTilt = inputActions.playerMove.magnitude * lateralTiltAmount;

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation)) > 0.01f)
            {
                lateralTilt = Mathf.Clamp(playerVelocity.x * lateralTiltAmount, -lateralTiltAmount, lateralTiltAmount); // Inclinación lateral según el movimiento
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


    private void PlayerMove2()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (playerMovement.magnitude > 0.1f)
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, playerMovement * playerSpeed, Time.deltaTime * accelerationFactor);   
        }
        else
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, Vector3.zero, Time.deltaTime * decelerationFactor);
        }

        if (playerVelocity.magnitude > 0.1f)
        {
            frontalTilt = Mathf.Clamp(playerVelocity.z * frontalTiltAmount, -frontalTiltAmount, frontalTiltAmount);
            lateralTilt = Mathf.Clamp(playerVelocity.x * lateralTiltAmount, -lateralTiltAmount, lateralTiltAmount);
        }
        else
        {
            frontalTilt = 0.0f;
            lateralTilt = 0.0f;
        }

        float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, frontalTilt, Time.deltaTime * tiltSmooth);
        float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

        transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, -smoothedLateralTilt);

        ballRb.velocity = new Vector3(playerVelocity.x, ballRb.velocity.y, playerVelocity.z);
    }


    private void PlayerMove()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (playerMovement.magnitude > 0.1f)
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, playerMovement * playerSpeed, Time.deltaTime * accelerationFactor);
        }
        else
        {
            playerVelocity = Vector3.MoveTowards(playerVelocity, Vector3.zero, Time.deltaTime * decelerationFactor);
        }

        ballRb.velocity = new Vector3(playerVelocity.x, ballRb.velocity.y, playerVelocity.z);

        // **Detectar si el personaje está girando**
        float targetRotation = transform.eulerAngles.y;

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la dirección a grados, en dirección a la cámara
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg; //+ mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se calcula la inclinación al moverse
            frontalTilt = Mathf.Clamp(playerVelocity.z * frontalTiltAmount, -frontalTiltAmount, frontalTiltAmount);
            lateralTilt = inputActions.playerMove.magnitude * lateralTiltAmount;

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation)) > 0.01f)
            {
                lateralTilt = Mathf.Clamp(playerVelocity.x * lateralTiltAmount, -lateralTiltAmount, lateralTiltAmount); // Inclinación lateral según el movimiento
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
        
    }
}
