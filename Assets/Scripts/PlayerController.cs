using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private GameObject playerAim;
    [SerializeField]
    private float wheelRadius;
    [SerializeField]
    private float playerSpeed = 5f;
    private float targetRotation;
    private float currentRotation;
    [SerializeField]
    private float turnSmoothTime = 0.25f;
    private float turnSmoothVelocity;
    [SerializeField]
    private float rotationSmoothSpeed = 0.1f;
    [SerializeField]
    private float tiltFactor = 5f;
    [SerializeField]
    private float tiltLimit = 25f;
    [SerializeField]
    private float tiltResetSpeed = 5f;
    private float frontalTilt;
    [SerializeField]
    private float frontalTiltSmoothSpeed = 0.1f;
    private float frontalTiltSmoothVelocity;
    private float lateralTilt;
    [SerializeField]
    private float lateralTiltSmoothSpeed = 0.1f;
    private float lateralTiltSmoothVelocity = 0.0f;

    [Header("Camera")]
    [SerializeField]
    private GameObject mainCamera;

    [Header("Shooting")]
    public GenericPool projectilePool;
    [SerializeField]
    private float shootRange = 100f;
    [SerializeField]
    private bool rechargeReady = true;
    [SerializeField]
    private float rechargeCooldown = 1f;
    private float rechargeTimer;


    private Animator animator;

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
        animator = playerBody.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        playerWheel.transform.position = ballRb.transform.position;
        playerBody.transform.position = new Vector3(playerWheel.transform.position.x, playerBody.transform.position.y, playerWheel.transform.position.z);

        if (!rechargeReady)
        {
            RechargeCooldown();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerShooting();
    }

    private void PlayerMovement()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (inputActions.playerMove.magnitude >= 0.1f)
        {
            float newTargetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            // Usamos SmoothDampAngle para evitar reseteos bruscos pero permitiendo cambios de dirección rápidos
            targetRotation = Mathf.SmoothDampAngle(targetRotation, newTargetRotation, ref turnSmoothVelocity, turnSmoothTime);
            currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }

        Vector3 playerVelocity = ballRb.velocity;
        float velocityMagnitude = new Vector2(playerVelocity.x, playerVelocity.z).magnitude;

        if (playerVelocity.magnitude >= 0.1f)
        { 
            float frontalTiltAmount = Mathf.Clamp(velocityMagnitude * tiltFactor, 0.0f, tiltLimit);
            frontalTilt = Mathf.SmoothDamp(frontalTilt, frontalTiltAmount, ref frontalTiltSmoothVelocity, frontalTiltSmoothSpeed);

            float rotationDifference = Mathf.DeltaAngle(currentRotation, targetRotation);
            float lateralTiltAmount = Mathf.Clamp(rotationDifference, -tiltLimit, tiltLimit);
            float tiltSpeedFactor = Mathf.Clamp01(velocityMagnitude / playerSpeed);
            lateralTilt = Mathf.SmoothDamp(lateralTilt, lateralTiltAmount * tiltSpeedFactor, ref lateralTiltSmoothVelocity, lateralTiltSmoothSpeed);
        }
        else
        {
            frontalTilt = Mathf.SmoothDamp(frontalTilt, 0.0f, ref frontalTiltSmoothVelocity, tiltResetSpeed);
            lateralTilt = Mathf.SmoothDamp(lateralTilt, 0.0f, ref lateralTiltSmoothVelocity, tiltResetSpeed);
        }

        playerWheel.transform.rotation = Quaternion.Euler(playerWheel.transform.rotation.eulerAngles.x, currentRotation, -lateralTilt * 0.5f);
        playerBody.transform.rotation = Quaternion.Euler(frontalTilt, currentRotation, -lateralTilt);
    
        Vector3 newPlayerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward * inputActions.playerMove.magnitude;
        ballRb.AddForce(newPlayerMovement * playerSpeed);
    }

    private void PlayerShooting()
    {
        if (inputActions.playerShoot && rechargeReady)
        {
            animator.SetTrigger("Shoot");

            Vector3 shootDirection = playerAim.transform.forward;
            shootDirection.y = 0.0f;

            Ray ray = new Ray(playerAim.transform.position, shootDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, shootRange))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log(hit.collider.name);
                }
                else if (hit.collider.CompareTag("Obstacle"))
                {
                    Debug.Log(hit.collider.name);
                }
            }

            GameObject projectile = projectilePool.GetElementFromPool();
            projectile.SetActive(true);
            projectile.transform.position = playerAim.transform.position;
            projectile.GetComponent<ProjectileBehaviour>().ProjectileDirection(shootDirection);

            rechargeReady = false;
            rechargeTimer = rechargeCooldown;

            inputActions.playerShoot = false;

            Debug.DrawRay(playerAim.transform.position, shootDirection * shootRange, Color.red, 1f); 
        }
    }

    private void RechargeCooldown()
    {
        rechargeTimer -= Time.deltaTime;

        if (rechargeTimer <= 0)
        {
            rechargeReady = true;
        }
    }
}
