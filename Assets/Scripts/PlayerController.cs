using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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
    private MeshRenderer wheelRenderer;
    private Material[] wheelMaterials;
    private Color movingNeonColor;
    private Color movingNeonEmission;
    [SerializeField]
    private float colorChangeSpeed;
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
    private bool rechargeReady = true;
    [SerializeField]
    private float rechargeCooldown = 1.5f;
    private float rechargeTimer;
    [SerializeField]
    private float detectionRadius = 25f;
    [SerializeField]
    private float shootingDecreasingPoints = 5f;

    private Animator animator;

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
        animator = playerBody.GetComponent<Animator>();
        wheelRenderer = playerWheel.GetComponent<MeshRenderer>();
        wheelMaterials = wheelRenderer.materials;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        movingNeonColor = wheelMaterials[2].color;
        movingNeonEmission = wheelMaterials[2].GetColor("_EmissionColor");
    }

    private void Update()
    {
        playerWheel.transform.position = ballRb.transform.position;
        playerBody.transform.position = playerWheel.transform.position;

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

        WheelColor();
    
        Vector3 newPlayerMovement = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward * inputActions.playerMove.magnitude;
        ballRb.AddForce(newPlayerMovement * playerSpeed);
    }

    private void WheelColor()
    {
        float velocityMagnitude = new Vector3(ballRb.velocity.x, 0.0f, ballRb.velocity.z).magnitude;
        colorChangeSpeed = Mathf.Clamp01(velocityMagnitude / playerSpeed * 2f);

        Color newNeonColor = Color.Lerp(Color.black, movingNeonColor, colorChangeSpeed);
        Color newNeonEmission = Color.Lerp(Color.black, movingNeonEmission, colorChangeSpeed);
        Color newDetailsColor = Color.Lerp(Color.black, movingNeonColor, colorChangeSpeed);

        wheelMaterials[2].SetColor("_BaseColor", newNeonColor);
        wheelMaterials[2].SetColor("_EmissionColor", newNeonEmission);
        wheelMaterials[1].SetColor("_BaseColor", newDetailsColor);

        wheelRenderer.materials = wheelMaterials;
    }

    private void PlayerShooting()
    {
        if (inputActions.playerShoot && rechargeReady)
        {
            animator.SetTrigger("Shoot");

            Transform targetEnemy = SelectEnemy();

            GameObject projectile = projectilePool.GetElementFromPool();
            projectile.SetActive(true);
            projectile.transform.position = playerAim.transform.position;
            
            ProjectileBehaviour projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>();

            if (targetEnemy != null)
            {
                Vector3 shootDirection = (targetEnemy.position - playerAim.transform.position).normalized;

                projectileBehaviour.ProjectileTarget(targetEnemy);  

                Debug.DrawRay(playerAim.transform.position, shootDirection * 100f, Color.red, 1f);
            }
            else
            {
                Vector3 shootDirection = playerAim.transform.forward;
                shootDirection.y = 0.0f;

                projectileBehaviour.ProjectileDirection(shootDirection);

                Debug.DrawRay(playerAim.transform.position, shootDirection * 100f, Color.green, 1f);
            }

            rechargeReady = false;
            rechargeTimer = rechargeCooldown;

            UIManager.instance.ScoreManager(shootingDecreasingPoints);

            inputActions.playerShoot = false; 
        }
    }

    private Transform SelectEnemy()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(playerBody.transform.position, detectionRadius);

        Transform nearestEnemy = null;
        float detectionArea = Mathf.Infinity;

        foreach (var collider in enemyColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector3 enemyPosition = collider.transform.position + Vector3.up;
                float distanceToEnemy = Vector3.Distance(playerBody.transform.position, collider.transform.position + Vector3.up);

                // Verificar si hay un obstáculo entre el jugador y el enemigo
                if (!Physics.Linecast(playerBody.transform.position, enemyPosition, LayerMask.GetMask("Environment")))
                {
                    if (distanceToEnemy < detectionArea)
                    {
                        detectionArea = distanceToEnemy;
                        nearestEnemy = collider.transform;
                    }
                }
            }
        }

        return nearestEnemy;
    }

    private void RechargeCooldown()
    {
        rechargeTimer -= Time.deltaTime;

        if (rechargeTimer <= 0)
        {
            rechargeReady = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerBody.transform.position, detectionRadius);
    }
}
