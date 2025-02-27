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
    private Animator animator;
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
    public bool rechargeReady = true;
    [SerializeField]
    public float rechargeCooldown = 5f;
    [SerializeField]
    private float rechargeTimer;
    [SerializeField]
    private float detectionRadius = 25f;
    [SerializeField]
    private float shootingDecreasingPoints = -5f;

    [Header("Effects")]
    [SerializeField]
    private ParticleSystem leftCannonParticles;
    [SerializeField]
    private ParticleSystem rightCannonParticles;
    [SerializeField]
    private GameObject wheelTrail;

    /// <summary>
    /// Se obtienen las referencias de varios componentes
    /// </summary>
    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
        animator = playerBody.GetComponent<Animator>();
        wheelRenderer = playerWheel.GetComponent<MeshRenderer>();
        wheelMaterials = wheelRenderer.materials;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Se obtienen las referencias de los colores iniciales de la rueda
    /// </summary>
    private void Start()
    {
        movingNeonColor = wheelMaterials[2].color;
        movingNeonEmission = wheelMaterials[2].GetColor("_EmissionColor");
    }

    private void Update()
    {
        playerWheel.transform.position = ballRb.transform.position;
        playerBody.transform.position = new Vector3 (playerWheel.transform.position.x, playerWheel.transform.position.y + 0.075f, playerWheel.transform.position.z);
        wheelTrail.transform.rotation = Quaternion.Euler(0f, playerWheel.transform.rotation.y, 0f);

        PlayerShooting();

        if (!rechargeReady)
        {
            RechargeCooldown();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    /// <summary>
    /// Función que controla el movimiento del jugador, según el rigidbody de la esfera. Movimiento hacia adelante, inclinacion frontal según la velocidad e
    /// inclinación lateral según el ángulo de giro. Rotación del jugador según la dirección de movimiento y de la cámara
    /// </summary>
    private void PlayerMovement()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        if (inputActions.playerMove.magnitude >= 0.1f)
        {
            float newTargetRotation = Mathf.Atan2(playerMovement.x, playerMovement.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

            targetRotation = Mathf.SmoothDampAngle(targetRotation, newTargetRotation, ref turnSmoothVelocity, turnSmoothTime);
            currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
        else
        {
            float newTargetRotation = mainCamera.transform.eulerAngles.y;

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

    /// <summary>
    /// Función que controla el color de la rueda, según la velocidad del jugador
    /// </summary>
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

    /// <summary>
    /// Función que controla el disparo del jugador, dando una dirección inicial específica según si hay un enemigo o no
    /// </summary>
    private void PlayerShooting()
    {
        if (inputActions.playerShoot && rechargeReady && EnergySystem.instance.canShoot == true)
        {
            animator.SetTrigger("Shoot");
            CameraController.instance.CameraShake();
            AudioManager.instance.PlayOneShot(AudioManager.instance.shootingSound);
            AudioManager.instance.PlaySound(AudioManager.instance.shootingCooldownSound);

            Transform targetEnemy = SelectEnemy();

            GameObject projectile = projectilePool.GetElementFromPool();
            projectile.transform.position = playerAim.transform.position;
            projectile.transform.rotation = playerAim.transform.rotation;
            projectile.SetActive(true);

            ProjectileBehaviour projectileBehaviour = projectile.GetComponent<ProjectileBehaviour>();

            if (targetEnemy != null)
            {
                //Vector3 shootDirection = (targetEnemy.position - playerAim.transform.position).normalized;

                projectileBehaviour.ProjectileTarget(targetEnemy);  

                //Debug.DrawRay(playerAim.transform.position, shootDirection * 100f, Color.red, 1f);
            }
            else
            {
                Vector3 shootDirection = projectile.transform.forward;
                shootDirection.y = 0.0f;

                projectileBehaviour.ProjectileDirection(shootDirection);

                //Debug.DrawRay(playerAim.transform.position, shootDirection * 100f, Color.green, 1f);
            }

            rechargeReady = false;
            rechargeTimer = rechargeCooldown;

            EnergySystem.instance.EnergyRecharge(-EnergySystem.instance.energyPerShoot);

            leftCannonParticles.Play();
            rightCannonParticles.Play();

            UIManager.instance.ScoreManager(shootingDecreasingPoints);

            inputActions.playerShoot = false; 
        }
    }

    /// <summary>
    /// Función que controla el cooldown del disparo del jugador
    /// </summary>
    private void RechargeCooldown()
    {
        rechargeTimer -= Time.deltaTime;

        if (rechargeTimer <= 0)
        {
            rechargeReady = true;
            EnergySystem.instance.currentEnergyCooldown = rechargeCooldown;

            leftCannonParticles.Stop();
            rightCannonParticles.Stop();
            AudioManager.instance.StopSound(AudioManager.instance.shootingCooldownSound);
        }
    }

    /// <summary>
    /// Función que, mediante el uso de OverlapSphere y Linecast, obtiene una lista de enemigos (que no estén detrás de obstáculos), marcando al más cercano al jugador
    /// </summary>
    /// <returns></returns>
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Energy Recharge"))
        {
            EnergySystem.instance.energyRechargePowerUpActive = true;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Instant Energy"))
        {
            Destroy(other.gameObject);
            EnergySystem.instance.currentEnergy = 100f;
        }
    }

    /// <summary>
    /// Función que dibuja una esfera, para saber el rango de detección de enemigos
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerBody.transform.position, detectionRadius);
    }
}
