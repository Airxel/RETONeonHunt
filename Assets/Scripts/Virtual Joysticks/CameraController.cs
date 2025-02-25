using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineFreeLook freeLookCamera;
    [SerializeField]
    private InputActions inputActions;
    [SerializeField]
    private float xRotationSensitivity = 0.05f;
    [SerializeField]
    private float yRotationSensitivity = 0.01f;

    private void Update()
    {
        Vector2 lookInput = inputActions.playerLook;

        freeLookCamera.m_XAxis.Value += lookInput.x * xRotationSensitivity * Time.deltaTime;
        freeLookCamera.m_YAxis.Value += lookInput.y * yRotationSensitivity * Time.deltaTime;
    }
}
