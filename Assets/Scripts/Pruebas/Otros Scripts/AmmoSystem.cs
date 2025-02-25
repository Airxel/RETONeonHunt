using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSystem : MonoBehaviour
{
    [SerializeField]
    public int maxAmmo = 10;
    [SerializeField]
    public int currentAmmo;

    public void ModifyAmmo(int amount)
    {
        if (currentAmmo + amount <= 0)
        {
            Debug.Log("Sin munición");
            currentAmmo = 0;
        }
        else if (currentAmmo + amount > maxAmmo)
        {
            Debug.Log("Munición al máximo");
            currentAmmo = maxAmmo;
        }
        else
        {
            Debug.Log(amount + " de munición añadida");
            currentAmmo += amount;
        }
    }
}
