using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootAbility : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public GenericPool projectilePool;

    private void Awake()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        CheckShoot();
    }

    private void CheckShoot()
    {
        if (_input.shoot)
        {
            Debug.Log("SHOOTING");

            GameObject projectile = projectilePool.GetElementFromPool();
            projectile.SetActive(true);
            projectile.transform.position = transform.position + transform.up * 0.5f + transform.forward * 0.5f;

            // Soltar el botón
            _input.shoot = false;
        }
    }
}
