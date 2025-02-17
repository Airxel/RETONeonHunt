using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPool : MonoBehaviour
{
    // Se crea la variable de la pool de objetos
    Stack<GameObject> _pool = new Stack<GameObject>();

    [SerializeField]
    private GameObject prefabToInstantiate;

    [SerializeField]
    private int poolSize;

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateElement();
        }
    }

    GameObject CreateElement()
    {
        GameObject temporalElement = Instantiate(prefabToInstantiate, Vector3.zero, Quaternion.identity);

        temporalElement.GetComponent<ProjectileBehaviour>().projectilePool = this;

        // Se carga la piscina con elementos
        _pool.Push(temporalElement);
        temporalElement.SetActive(false);

        return temporalElement;
    }

    /// <summary>
    /// Crear y sacar el elemento de la pool
    /// </summary>
    /// <returns></returns>
    public GameObject GetElementFromPool()
    {
        GameObject toReturn = null;

        if (_pool.Count == 0)
        {
            CreateElement();
        }
        else
        {
            toReturn = _pool.Pop();
        }
        return toReturn;
    }

    /// <summary>
    /// Devolver el elemento a la pool
    /// </summary>
    /// <param name="element"></param>
    public void ReturnToPool(GameObject element)
    {
        element.SetActive(false);
        _pool.Push(element);
    }
}
