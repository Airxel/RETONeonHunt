using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
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
    

    //[SerializeField]
    //private float tiltAmount;
    //[SerializeField]
    //private float tiltSmooth;

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

    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float tiltAmount = 15f;
    public float tiltSmooth = 5f;

    private Vector3 moveDirection;
    private float currentSpeed = 0.0f;
    private float targetTilt = 0.0f;
    //private float lateralTilt = 0.0f;
    //private float rotationVelocity = 0.0f;

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
        OtherPlayerMovement();
    }

    private void PlayerMove()
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

        targetRotation = transform.eulerAngles.y;

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la direcci�n a grados, en direcci�n a la c�mara
            targetRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg; //+ mainCamera.transform.eulerAngles.y;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // Se calcula la inclinaci�n al moverse
            frontalTilt = inputActions.playerMove.magnitude * tiltAmount;
            lateralTilt = inputActions.playerMove.magnitude * tiltAmount;

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation)) > 0.01f)
            {
                lateralTilt = inputActions.playerMove.magnitude * tiltAmount; // Inclinaci�n lateral seg�n el movimiento
            }
            else
            {
                lateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, 0.0f, Time.deltaTime * tiltSmooth);
            }
            
            float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, frontalTilt, Time.deltaTime * tiltSmooth);
            float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

            // Se aplica la inclinaci�n y rotaci�n
            transform.rotation = Quaternion.Euler(smoothedFrontalTilt, playerRotation, smoothedLateralTilt);
        }
        else
        {
            // Si no hay movimiento, se hace una interpolaci�n para devolver las inclinaciones a 0
            float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, 0.0f, Time.deltaTime * tiltSmooth);
            float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, 0.0f, Time.deltaTime * tiltSmooth);

            // Aplicar la rotaci�n sin inclinaci�n
            transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, smoothedLateralTilt);
        }

        // Se mueve el jugador seg�n la direcci�n en la que mire
        Vector3 playerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(playerMovement * speedAcceleration * Time.deltaTime);
    }

    private void Move()
    {

        Vector2 input = inputActions.playerMove;
        moveDirection = new Vector3(input.x, 0.0f, input.y).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0.0f, moveSpeed);
            targetTilt = -tiltAmount; // Inclinaci�n hacia adelante
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0.0f);
            targetTilt = 0.0f; // Volver a posici�n original
        }

        Vector3 movement = moveDirection * currentSpeed;
        characterController.Move(movement * Time.deltaTime);

        // Detectar si el personaje est� girando
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg));

        if (angleDifference > 0.1f)
        {
            lateralTilt = tiltAmount * Mathf.Sign(Mathf.DeltaAngle(transform.eulerAngles.y, Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg));
        }
        else
        {
            lateralTilt = 0.0f;
        }

        // Aplicar inclinaciones suavemente
        float smoothedFrontalTilt = Mathf.LerpAngle(transform.localEulerAngles.x, targetTilt, Time.deltaTime * tiltSmooth);
        float smoothedLateralTilt = Mathf.LerpAngle(transform.localEulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);

        // Aplicar inclinaci�n
        transform.rotation = Quaternion.Euler(smoothedFrontalTilt, transform.eulerAngles.y, smoothedLateralTilt);

        // Rotar en la direcci�n del movimiento
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
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

    private void OtherPlayerMovement()
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

        // Variaci�n si se usa un joystick (depende de cu�nto se mueva)
        joystickInput = inputActions.analogMovement ? inputActions.playerMove.magnitude : 1f;

        // Se controla la aceleraci�n del jugador
        speedAcceleration = Mathf.Lerp(playerVelocity, newPlayerSpeed * joystickInput, Time.deltaTime * speedChangeFactor);

        // Se obtiene la direcci�n de movimiento del jugador en el mundo
        Vector3 playerDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        float targetRotation = transform.eulerAngles.y; // Mantener la rotaci�n

        if (inputActions.playerMove != Vector2.zero)
        {
            // Se convierte la direcci�n a grados y la ajustamos con la c�mara
            targetRotation = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg;

            float playerRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // **Obtenemos el vector de movimiento real**
            Vector3 playerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            // **Inclinaci�n basada en el movimiento**
            frontalTilt = -playerMovement.z * tiltAmount; // Se inclina en la direcci�n del movimiento
            lateralTilt = playerMovement.x * tiltAmount;  // Se inclina lateralmente seg�n el movimiento
        }
        else
        {
            // Si el jugador est� quieto, la inclinaci�n vuelve a 0
            frontalTilt = 0f;
            lateralTilt = 0f;
        }

        // Suavizamos la inclinaci�n para evitar cambios bruscos
        float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, frontalTilt, Time.deltaTime * tiltSmooth);
        float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, lateralTilt, Time.deltaTime * tiltSmooth);
        float smoothedRotation = Mathf.LerpAngle(transform.eulerAngles.y, targetRotation, Time.deltaTime * rotationSmooth);

        // Aplicamos la rotaci�n e inclinaci�n
        transform.rotation = Quaternion.Euler(smoothedFrontalTilt, smoothedRotation, smoothedLateralTilt);

        // Se mueve el jugador seg�n la direcci�n en la que mire
        Vector3 movement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        characterController.Move(movement * speedAcceleration * Time.deltaTime);
    }

}