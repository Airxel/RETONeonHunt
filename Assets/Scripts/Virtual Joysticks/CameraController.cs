using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private CinemachineFreeLook freeLookCamera;
    [SerializeField]
    private CinemachineImpulseSource impulseSource;
    [SerializeField]
    private InputActions inputActions;
    [SerializeField]
    private float xRotationSensitivity = 0.05f;
    [SerializeField]
    private float yRotationSensitivity = 0.01f;
    [SerializeField]
    private GameObject playerBody;

    //Singleton
    public static CameraController instance;

    private void Awake()
    {
        if (CameraController.instance == null)
        {
            CameraController.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Funci�n que controla la rotaci�n de la c�mara cuando se usa el joystick virtual, seg�n el input
    /// </summary>
    private void Update()
    {
        Vector2 lookInput = inputActions.playerLook;
        //freeLookCamera.transform.position = playerBody.transform.position;

        freeLookCamera.m_XAxis.Value += lookInput.x * xRotationSensitivity * Time.deltaTime;
        freeLookCamera.m_YAxis.Value += lookInput.y * yRotationSensitivity * Time.deltaTime;

        //freeLookCamera.LookAt = playerBody.transform;
        //freeLookCamera.Follow = playerBody.transform;
    }

    /// <summary>
    /// Funci�n que realiza un shake de la c�mara
    /// </summary>
    public void CameraShake()
    {
        impulseSource.GenerateImpulse();
    }
}
