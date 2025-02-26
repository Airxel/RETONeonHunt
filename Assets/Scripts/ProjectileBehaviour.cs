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

    [Header("Effects")]
    [SerializeField]
    private GameObject cubeBurstParticles;

    /// <summary>
    /// Función que controla el movimiento y el tiempo del proyectil. Si hay un enemigo detectado, cambia de dirección continuamente hacia el enemigo
    /// </summary>
    private void Update()
    {
        if (projectileTarget != null)
        {
            Vector3 targetDirection = (projectileTarget.position - transform.position).normalized;
            projectileDirection = Vector3.Lerp(transform.forward, targetDirection, homingStrength * Time.deltaTime);
        }

        transform.forward = projectileDirection;
        projectileDirection.y = 0f;
        transform.position += projectileDirection * projectileSpeed * Time.deltaTime;

        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
        {
            projectilePool.ReturnToPool(gameObject);
        }
    }

    /// <summary>
    /// Función que obtiene la dirección desde donde se dispara el proyectil, si no hay enemigo detectado
    /// </summary>
    /// <param name="shootDirection"></param>
    public void ProjectileDirection(Vector3 shootDirection)
    {
        projectileDirection = shootDirection;
        currentLifetime = lifetime;
        projectileTarget = null;
    }

    /// <summary>
    /// Función que obtiene si hay un enemigo detectado o no, una vez se ejecute un disparo
    /// </summary>
    /// <param name="newProjectileTarget"></param>
    public void ProjectileTarget(Transform newProjectileTarget)
    {
        projectileTarget = newProjectileTarget;
        currentLifetime = lifetime;
    }

    /// <summary>
    /// Función que controla como interactúa el proyectil con el entorno y los enemigos, devolviéndolo a la pool
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemyInstance = other.transform.parent.gameObject;

            Vector3 spawnPosition = other.transform.position + Vector3.up;
            Instantiate(cubeBurstParticles, spawnPosition, Quaternion.identity);
            enemyInstance.SetActive(false);

            UIManager.instance.ScoreManager(enemyIncreasingPoints);
            UIManager.instance.EnemiesManager(-1);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Vector3 spawnPosition = other.ClosestPoint(transform.position);
            Instantiate(cubeBurstParticles, spawnPosition, Quaternion.identity);
        }
        
        projectilePool.ReturnToPool(gameObject);
    }
}
