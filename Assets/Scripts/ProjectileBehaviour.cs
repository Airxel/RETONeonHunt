using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public GenericPool projectilePool;
    private Vector3 projectileDirection;
    float timer = 5f;

    [SerializeField]
    float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.position += projectileDirection * speed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            projectilePool.ReturnToPool(gameObject);
            timer = 5f;
        }
    }

    public void ProjectileDirection(Vector3 direction)
    {
        projectileDirection = direction.normalized;
        transform.forward = direction;
        timer = 5f;
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
