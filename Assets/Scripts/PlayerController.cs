using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private float playerSpeed;
    private float playerVelocity;
    [SerializeField]
    private float speedAcceleration;
    private float joystickInput;

    [SerializeField]
    private float speedChangeFactor;
    private float targetRotation;
    private float rotationVelocity;
    [SerializeField]
    private float rotationSmooth;

    private float frontalTilt;
    private float lateralTilt;
    

    [SerializeField]
    private float tiltAmount;
    [SerializeField]
    private float tiltSmooth;

    [Header("Camera")]
    [SerializeField]
    private GameObject cameraTarget;
    [SerializeField]
    private float topCameraLimit;
    [SerializeField]
    private float bottomCameraLimit;
    private float cameraTargetYaw;
    private float cameraTargetPitch;

    private const float cameraThreshold = 0.01f;
    public bool lockCameraPosition = false;
    public float cameraAngleOverride = 0.0f;

    private PlayerInput playerInput;
    private InputActions inputActions;
    private CharacterController characterController;
    private GameObject mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        inputActions = GetComponent<InputActions>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        // Se guarda la velocidad actual
        float newPlayerSpeed = playerSpeed;

        // Si no hay inputs, no se mueve
        if (inputActions.playerMove == Vector2.zero)
        {
            newPlayerSpeed = 0.0f;
        }

        // Se guarda la velocity actual del Character Controller
        playerVelocity = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        // Variación si se usa un joystick (depende de cuanto se mueva)
        if (inputActions.analogMovement)
        {
            joystickInput = inputActions.playerMove.magnitude;
        }
        else
        {
            joystickInput = 1f;
        }

        // Valor para que la aceleración se mantenga sin variar
        float speedOffset = 0.1f;

        // Se controla la aceleración del jugador
        if (playerVelocity < newPlayerSpeed - speedOffset || playerVelocity > newPlayerSpeed + speedOffset)
        {
            speedAcceleration = Mathf.Lerp(playerVelocity, newPlayerSpeed * joystickInput, Time.deltaTime * speedChangeFactor);
        }
        else
        {
            speedAcceleration = newPlayerSpeed;
        }

        // Se obtiene la dirección de movimiento del jugador
        Vector3 playerDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y);

        targetRotation = transform.eulerAngles.y;

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la dirección a grados, en dirección a la cámara
            targetRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg; //+ mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se calcula la inclinación al moverse
            frontalTilt = inputActions.playerMove.magnitude * tiltAmount;

            lateralTilt = 0.0f;

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation)) > 0.1f)
            {
                lateralTilt = inputActions.playerMove.magnitude * tiltAmount; // Inclinación lateral según el movimiento
            }
            
            float smoothedFrontalTilt = Mathf.LerpAngle(transform.localEulerAngles.x, frontalTilt, Time.deltaTime * tiltSmooth);
            float smoothedLateralTilt = Mathf.LerpAngle(transform.localEulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

            // Se aplica la inclinación y rotación
            transform.rotation = Quaternion.Euler(smoothedFrontalTilt, playerRotation, smoothedLateralTilt);
        }
        else
        {
            // Si no hay movimiento, se hace una interpolación para devolver las inclinaciones a 0
            float smoothedFrontalTilt = Mathf.LerpAngle(transform.localEulerAngles.x, 0.0f, Time.deltaTime * tiltSmooth);
            float smoothedLateralTilt = Mathf.LerpAngle(transform.localEulerAngles.z, 0.0f, Time.deltaTime * tiltSmooth);

            // Aplicar la rotación sin inclinación
            transform.localRotation = Quaternion.Euler(smoothedFrontalTilt, transform.localEulerAngles.y, smoothedLateralTilt);
        }

        // Se mueve el jugador según la dirección en la que mire
        Vector3 playerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(playerMovement * speedAcceleration * Time.deltaTime);
    }

    private void NewPlayerMovement()
    {
        // Se guarda la velocidad actual
        float newPlayerSpeed = playerSpeed;

        // Si no hay inputs, no se mueve
        if (inputActions.playerMove == Vector2.zero)
        {
            newPlayerSpeed = 0.0f;
        }

        // Se guarda la velocity actual del Character Controller
        playerVelocity = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        // Variación si se usa un joystick (depende de cuanto se mueva)
        if (inputActions.analogMovement)
        {
            joystickInput = inputActions.playerMove.magnitude;
        }
        else
        {
            joystickInput = 1f;
        }

        // Valor para que la aceleración se mantenga sin variar
        float speedOffset = 0.1f;

        // Se controla la aceleración del jugador
        if (playerVelocity < newPlayerSpeed - speedOffset || playerVelocity > newPlayerSpeed + speedOffset)
        {
            speedAcceleration = Mathf.Lerp(playerVelocity, newPlayerSpeed * joystickInput, Time.deltaTime * speedChangeFactor);
        }
        else
        {
            speedAcceleration = newPlayerSpeed;
        }

        // Se obtiene la dirección de movimiento del jugador
        Vector3 playerDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y);

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la dirección a grados, en dirección a la cámara
            targetRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se aplica la rotación
            transform.rotation = Quaternion.Euler(0.0f, playerRotation, 0.0f);
        }

        // Se mueve el jugador según la dirección en la que mire
        Vector3 playerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(playerMovement * speedAcceleration * Time.deltaTime);
    }
}