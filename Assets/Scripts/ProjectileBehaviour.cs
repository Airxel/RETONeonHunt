using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("Projectile")]
    public GenericPool projectilePool;
    private Vector3 projectileDirection;
    [SerializeField]
    private Transform projectileTarget;
    [SerializeField]
    float projectileSpeed = 10f;
    [SerializeField]
    private float homingStrength = 10f;
    [SerializeField]
    private float lifetime = 5f;
    private float currentLifetime;

    private void Update()
    {
        if (projectileTarget != null)
        {
            projectileDirection = (projectileTarget.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, projectileDirection, homingStrength * Time.deltaTime);
        }

        transform.position += projectileDirection * projectileSpeed * Time.deltaTime;

        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
        {
            projectilePool.ReturnToPool(gameObject);
        }
    }

    public void ProjectileDirection(Vector3 shootDirection)
    {
        projectileDirection = shootDirection;
        currentLifetime = lifetime;
        projectileTarget = null;
    }

    public void ProjectileTarget(Transform newProjectileTarget)
    {
        projectileTarget = newProjectileTarget;
        currentLifetime = lifetime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemyInstance = other.transform.parent.gameObject;

            enemyInstance.SetActive(false);
        }

        projectilePool.ReturnToPool(gameObject);

        Debug.Log("Projectile hit: " + other.tag);
    }
}
