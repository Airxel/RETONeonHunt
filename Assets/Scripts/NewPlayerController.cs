using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    private Rigidbody ballRb;
    private InputActions inputActions;

    [SerializeField]
    private GameObject playerRobot;

    [SerializeField]
    private float playerSpeed = 5f;
    [SerializeField]
    private float tiltFactor = 5f;
    [SerializeField]
    private float tiltLimit = 25f;
    [SerializeField]
    private float tiltSmooth = 5f;

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody>();
        inputActions = GetComponent<InputActions>();
    }

    private void Update()
    {
        playerRobot.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 playerMovement = new Vector3(inputActions.playerMove.x, 0.0f, inputActions.playerMove.y).normalized;

        ballRb.AddForce(playerMovement * playerSpeed);

        Vector3 playerVelocity = ballRb.velocity;

        if (playerVelocity.magnitude > 0.1f)
        {
            float lateralTiltAngle = Mathf.Clamp(playerVelocity.x * tiltFactor, -tiltLimit, tiltLimit);
            float frontalTiltAngle = Mathf.Clamp(playerVelocity.z * tiltFactor, -tiltLimit, tiltLimit);

            //float smoothedFrontalTilt = Mathf.LerpAngle(transform.eulerAngles.x, frontalTiltAngle, Time.deltaTime * tiltSmooth);
            //float smoothedLateralTilt = Mathf.LerpAngle(transform.eulerAngles.z, lateralTiltAngle, Time.deltaTime * tiltSmooth);

            playerRobot.transform.rotation = Quaternion.Euler(frontalTiltAngle, 0, -lateralTiltAngle);
        }
        else
        {

        }
    }
}
