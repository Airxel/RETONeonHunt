using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public GenericPool projectilePool;
    float timer = 5f;

    [SerializeField]
    float speed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            projectilePool.ReturnToPool(gameObject);
            timer = 5f;
        }
    }
}
