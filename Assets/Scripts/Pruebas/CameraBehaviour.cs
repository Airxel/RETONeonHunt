using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    InputActions inputActions;
    public float sensibilidad = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        inputActions = FindAnyObjectByType<InputActions>();
    }

    // Update is called once per frame
    void Update()
    {
        float rotationX = inputActions.playerLook.x * sensibilidad;

        transform.localRotation = Quaternion.Euler(0, rotationX, 0);
    }
}
