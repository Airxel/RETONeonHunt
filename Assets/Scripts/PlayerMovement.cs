using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputActions inputActions;
    private Rigidbody ballRb;
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    //public float tiltAmount = 15f;  // Inclinación hacia adelante
    //public float tiltSmooth = 5f;   // Suavidad de la inclinación
    public float lateralTiltAmount = 10f; // Inclinación lateral al girar
    public float lateralTiltResetSpeed = 5f; // Velocidad de reseteo lateral

    private Vector3 moveDirection;
    private Vector3 velocity;
    private float targetTilt = 0.0f;
    //private float lateralTilt = 0.0f;
    private float previousRotation = 0.0f;
    private float currentSpeed = 0.0f;

    float tiltAmount = 25f;      // Cuánto se inclina el robot
    private float frontalTilt = 25f;
    private float lateralTilt = 25f;
    float maxTiltAngle = 20.0f;    // Máxima inclinación
    float tiltSmooth = 5.0f;       // Suavidad de inclinación
    float tiltResetSpeed = 2.0f;   // Velocidad de regreso a 0
    private float targetRotation;
    private float rotationVelocity;
    [SerializeField]
    private float rotationSmooth;

    [Header("Player")]
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float speedAcceleration;
    [SerializeField]
    private float accelerationFactor;
    [SerializeField]
    private float decelerationFactor;

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
            speedAcceleration = Mathf.Lerp(speedAcceleration, playerSpeed, Time.deltaTime * accelerationFactor);
        }
        else
        {
            speedAcceleration = Mathf.Lerp(speedAcceleration, 0.0f, Time.deltaTime * decelerationFactor);
        }

        Vector3 playerVelocity = playerMovement * speedAcceleration;
        ballRb.velocity = new Vector3(playerVelocity.x, ballRb.velocity.y, playerVelocity.z);

        if (playerVelocity.magnitude > 0.1f)
        {
            frontalTilt = -tiltAmount; // Inclinación hacia adelante
        }
        else
        {
            frontalTilt = 0.0f; // Volver a posición original
        }

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation)) > 0.01f)
        {
            lateralTilt = inputActions.playerMove.magnitude * tiltAmount; // Inclinación lateral según el movimiento
        }
        else
        {
            lateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, 0.0f, Time.deltaTime * tiltSmooth);
        }

        float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, frontalTilt, Time.deltaTime * tiltSmooth);
        float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

        // Aplicar inclinación
        transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, smoothedLateralTilt);

        // Rotar hacia la dirección de movimiento
        if (playerMovement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerMovement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        //ballRb.AddForce(playerMovement * speedAcceleration);
    }

    private void MoreMovement()
    {
        Vector2 input = inputActions.playerMove;
        moveDirection = new Vector3(input.x, 0.0f, input.y).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            velocity = Vector3.Lerp(velocity, moveDirection * moveSpeed, Time.deltaTime * acceleration);
            targetTilt = -tiltAmount; // Inclinación hacia adelante
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * deceleration);
            targetTilt = 0.0f; // Volver a posición original
        }

        ballRb.velocity = new Vector3(velocity.x, ballRb.velocity.y, velocity.z);

        // **Detectar si el personaje está girando**
        float currentRotation = transform.eulerAngles.y;
        float rotationSpeed = Mathf.Abs(Mathf.DeltaAngle(previousRotation, currentRotation)) / Time.deltaTime;

        if (rotationSpeed > 1.0f)  // Si está girando
        {
            lateralTilt = lateralTiltAmount * Mathf.Sign(Mathf.DeltaAngle(previousRotation, currentRotation));
        }
        else
        {
            lateralTilt = Mathf.Lerp(lateralTilt, 0.0f, Time.deltaTime * lateralTiltResetSpeed);
        }

        previousRotation = currentRotation; // Guardar rotación para la siguiente verificación

        // Aplicar inclinaciones suavemente
        float smoothedFrontalTilt = Mathf.LerpAngle(transform.localEulerAngles.x, targetTilt, Time.deltaTime * tiltSmooth);
        float smoothedLateralTilt = Mathf.LerpAngle(transform.localEulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

        // Aplicar inclinación
        transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, smoothedLateralTilt);

        // Rotar hacia la dirección de movimiento
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void AnotherMove()
    {
        Vector2 input = inputActions.playerMove;
        moveDirection = new Vector3(input.x, 0.0f, input.y).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.deltaTime * deceleration);
        }

        velocity = moveDirection * currentSpeed;
        ballRb.velocity = new Vector3(velocity.x, ballRb.velocity.y, velocity.z);

        // **Evitar el cabezazo: la inclinación depende de la velocidad real**
        targetTilt = -tiltAmount * (currentSpeed / moveSpeed);  // Inclinación frontal según la velocidad

        // **Detectar si el personaje está girando**
        float currentRotation = transform.eulerAngles.y;
        float rotationSpeed = Mathf.Abs(Mathf.DeltaAngle(previousRotation, currentRotation)) / Time.deltaTime;

        if (rotationSpeed > 1.0f)  // Si está girando
        {
            lateralTilt = lateralTiltAmount * Mathf.Sign(Mathf.DeltaAngle(previousRotation, currentRotation));
        }
        else
        {
            lateralTilt = Mathf.Lerp(lateralTilt, 0.0f, Time.deltaTime * lateralTiltResetSpeed);
        }

        previousRotation = currentRotation; // Guardar rotación para la siguiente verificación

        // Aplicar inclinaciones suavemente
        float smoothedFrontalTilt = Mathf.LerpAngle(transform.localEulerAngles.x, targetTilt, Time.deltaTime * tiltSmooth);
        float smoothedLateralTilt = Mathf.LerpAngle(transform.localEulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

        // Aplicar inclinación
        transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, smoothedLateralTilt);

        // Rotar hacia la dirección de movimiento
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
