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
    float speed = 10f;
    [SerializeField]
    private float homingStrength = 10f;
    [SerializeField]
    private float lifetime = 5f;

    private void Update()
    {
        if (projectileTarget != null)
        {
            projectileDirection = (projectileTarget.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, projectileDirection, homingStrength * Time.deltaTime);
        }

        transform.position += projectileDirection * speed * Time.deltaTime;

        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            projectilePool.ReturnToPool(gameObject);
        }
    }

    public void ProjectileDirection(Vector3 direction)
    {
        projectileDirection = direction.normalized;
        transform.forward = direction;
        lifetime = 5f;
        projectileTarget = null;
    }

    public void ProjectileTarget(Transform newProjectileTarget)
    {
        projectileTarget = newProjectileTarget;
        lifetime = 5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        {
            Debug.Log("Projectile hit: " + other.tag);

            projectilePool.ReturnToPool(gameObject);
        }
    }
}
