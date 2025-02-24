using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;  // Arrastra tu FreeLook aquí
    public InputActions inputActions; // Referencia al script donde guardas el input del joystick
    public float sensitivity = 10f; // Ajusta la sensibilidad del movimiento

    void Update()
    {
        // Asegurar que Cinemachine está referenciado
        if (freeLookCamera == null || inputActions == null) return;

        // Tomar los valores del joystick
        Vector2 lookInput = inputActions.playerLook;

        // Aplicar los valores a Cinemachine
        freeLookCamera.m_XAxis.Value += lookInput.x * sensitivity * Time.deltaTime;
        freeLookCamera.m_YAxis.Value += lookInput.y * sensitivity * Time.deltaTime;

        Debug.Log("Joystick Look Input: " + inputActions.playerLook);
    }
}
