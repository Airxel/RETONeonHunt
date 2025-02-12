using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody ballRb;
    private float playerMovementX;
    private float playerMovementY;

    [Header("Player")]
    [SerializeField]
    public float playerSpeed;
    private float playerVelocity;
    [SerializeField]
    private float speedAcceleration;
    private float joystickInput;

    [SerializeField]
    public float speedChangeFactor;
    private float targetRotation;
    private float rotationVelocity;
    [SerializeField]
    public float rotationSmooth;
    

    [SerializeField]
    private float tiltAmount;
    [SerializeField]
    private float tiltSmooth;

    private float playerTilt;
    private float tiltAngle;

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

    private bool IsCurrentDeviceMouse
    {
        get { return playerInput.currentControlScheme == "KeyboardMouse"; }
    }

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
        cameraTargetYaw = cameraTarget.transform.rotation.eulerAngles.y;

        playerInput = GetComponent<PlayerInput>();
        inputActions = GetComponent<InputActions>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void LateUpdate()
    {
        CameraRotation();
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

        // Variaci�n si se usa un joystick (depende de cuanto se mueva)
        if (inputActions.analogMovement)
        {
            joystickInput = inputActions.playerMove.magnitude;
        }
        else
        {
            joystickInput = 1f;
        }

        // Valor para que la aceleraci�n se mantenga sin variar
        float speedOffset = 0.1f;

        // Se controla la aceleraci�n del jugador
        if (playerVelocity < newPlayerSpeed - speedOffset || playerVelocity > newPlayerSpeed + speedOffset)
        {
            speedAcceleration = Mathf.Lerp(playerVelocity, newPlayerSpeed * joystickInput, Time.deltaTime * speedChangeFactor);
        }
        else
        {
            speedAcceleration = newPlayerSpeed;
        }

        // Se obtiene la direcci�n de movimiento del jugador
        Vector3 playerDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y);

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la direcci�n a grados, en direcci�n a la c�mara
            targetRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg; //+ mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // **C�lculo de inclinaci�n**
            float tiltX = -inputActions.playerMove.y * tiltAmount; // Adelante (+) / Atr�s (-)
            float tiltZ = inputActions.playerMove.x * tiltAmount;  // Izquierda (+) / Derecha (-)

            // Aplicamos la suavizaci�n para transiciones suaves
            float smoothedTiltX = Mathf.LerpAngle(transform.eulerAngles.x, tiltX, Time.deltaTime * tiltSmooth);
            float smoothedTiltZ = Mathf.LerpAngle(transform.eulerAngles.z, tiltZ, Time.deltaTime * tiltSmooth);

            // Aplicar inclinaci�n y rotaci�n
            transform.rotation = Quaternion.Euler(smoothedTiltX, playerRotation, smoothedTiltZ);
        }

        // Se mueve el jugador seg�n la direcci�n en la que mire
        Vector3 playerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(playerMovement * speedAcceleration * Time.deltaTime);
    }

    private void CameraRotation()
    {
        if (inputActions.playerLook.sqrMagnitude >= cameraThreshold && !lockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cameraTargetYaw += inputActions.playerLook.x;
            cameraTargetPitch += inputActions.playerLook.y;
        }

        // clamp our rotations so our values are limited 360 degrees
        cameraTargetYaw = ClampAngle(cameraTargetYaw, float.MinValue, float.MaxValue);
        cameraTargetPitch = ClampAngle(cameraTargetPitch, bottomCameraLimit, topCameraLimit);

        // Cinemachine will follow this target
        cameraTarget.transform.rotation = Quaternion.Euler(cameraTargetPitch + cameraAngleOverride, cameraTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OldMove()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y);

        ballRb.AddForce(playerMovement * playerSpeed);

        Vector3 playerVelocity = ballRb.velocity;

        if (playerVelocity.magnitude > 0.1f)
        {
            tiltAngle = Mathf.Clamp(playerVelocity.x * playerTilt, -playerTilt, playerTilt);
        }

        ballRb.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, -tiltAngle);
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

        // Variaci�n si se usa un joystick (depende de cuanto se mueva)
        if (inputActions.analogMovement)
        {
            joystickInput = inputActions.playerMove.magnitude;
        }
        else
        {
            joystickInput = 1f;
        }

        // Valor para que la aceleraci�n se mantenga sin variar
        float speedOffset = 0.1f;

        // Se controla la aceleraci�n del jugador
        if (playerVelocity < newPlayerSpeed - speedOffset || playerVelocity > newPlayerSpeed + speedOffset)
        {
            speedAcceleration = Mathf.Lerp(playerVelocity, newPlayerSpeed * joystickInput, Time.deltaTime * speedChangeFactor);
        }
        else
        {
            speedAcceleration = newPlayerSpeed;
        }

        // Se obtiene la direcci�n de movimiento del jugador
        Vector3 playerDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y);

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la direcci�n a grados, en direcci�n a la c�mara
            targetRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se aplica la rotaci�n
            transform.rotation = Quaternion.Euler(0.0f, playerRotation, 0.0f);
        }

        // Se mueve el jugador seg�n la direcci�n en la que mire
        Vector3 playerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(playerMovement * speedAcceleration * Time.deltaTime);
    }
}