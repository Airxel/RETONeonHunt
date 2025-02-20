using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField]
    private Transform enemyInstance;

    private void Start()
    {
        enemyInstance = transform.parent;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Projectile"))
        {
            Destroy(enemyInstance);

            Debug.Log("Got HIT!");
        }
    }
}
