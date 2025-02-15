using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationsController : MonoBehaviour
{
    [SerializeField]
    private GameObject robotBody;
    private Animator animator;
    private InputActions inputActions;
    private void Awake()
    {
        inputActions = GetComponent<InputActions>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        OnShoot();
    }

    private void OnShoot()
    {
        if (inputActions.playerShoot)
        {
            Debug.Log("Shooting");

            animator.SetTrigger("Shoot");

            inputActions.playerShoot = false;
        }
    }
}
