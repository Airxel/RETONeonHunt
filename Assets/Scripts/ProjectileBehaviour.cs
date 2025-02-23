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
    float projectileSpeed = 15f;
    [SerializeField]
    private float homingStrength = 10f;
    [SerializeField]
    private float lifetime = 5f;
    private float currentLifetime;
    [SerializeField]
    private float enemyIncreasingPoints = 10f;

    private void Update()
    {
        if (projectileTarget != null)
        {
            Vector3 targetDirection = (projectileTarget.position - transform.position).normalized;
            projectileDirection = Vector3.Lerp(transform.forward, targetDirection, homingStrength * Time.deltaTime);
        }

        transform.forward = projectileDirection;
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

            UIManager.instance.ScoreManager(enemyIncreasingPoints);
            UIManager.instance.EnemiesManager(-1);
        }

        projectilePool.ReturnToPool(gameObject);

        Debug.Log("Projectile hit: " + other.tag);
    }
}
