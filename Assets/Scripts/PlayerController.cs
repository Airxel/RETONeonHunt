using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
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
    private float speed;
    private float targetRotation;
    private float rotationVelocity;
    private float verticalVelocity;
    [SerializeField]
    public float rotationSmooth;
    [SerializeField]
    public float speedChangeRate;

    [SerializeField]
    public float playerTilt;
    private float tiltAngle;

    [Header("Camera")]
    [SerializeField]
    private GameObject CameraTarget;
    [SerializeField]
    private float topCameraLimit;
    [SerializeField]
    private float bottomCameraLimit;
    private float cameraTargetYaw;
    private float cameraTargetPitch;

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
        cameraTargetYaw = CameraTarget.transform.rotation.eulerAngles.y;

        playerInput = GetComponent<PlayerInput>();
        inputActions = GetComponent<InputActions>();
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        NewMove();
    }

    private void PlayerMovement()
    {
        float targetSpeed = playerSpeed;

        // if there is no input, set the target speed to 0
        if (inputActions.playerMove == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = inputActions.analogMovement ? inputActions.playerMove.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputActions.playerMove != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        // move the player
        characterController.Move(targetDirection.normalized * (speed * Time.deltaTime));
    }

    private void NewMove()
    {
        float newPlayerSpeed = playerSpeed;

        if (inputActions.playerMove == Vector2.zero)
        {
            newPlayerSpeed = 0.0f;
        }

        // normalise input direction
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y);

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputActions.playerMove != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmooth);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        // move the player
        characterController.Move(targetDirection * newPlayerSpeed * Time.deltaTime);
    }

    private void OldMove()
    {
        Vector3 playerMovement = new Vector3(playerMovementX, 0.0f, playerMovementY);

        ballRb.AddForce(playerMovement * playerSpeed);

        Vector3 playerVelocity = ballRb.velocity;

        if (playerVelocity.magnitude > 0.1f)
        {
            tiltAngle = Mathf.Clamp(playerVelocity.x * playerTilt, -playerTilt, playerTilt);
        }

        ballRb.rotation = Quaternion.Euler(0.0f, transform.eulerAngles.y, -tiltAngle);
    }

    
}