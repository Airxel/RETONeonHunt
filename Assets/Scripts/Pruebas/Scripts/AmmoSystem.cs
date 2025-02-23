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
            Debug.Log("Sin munici�n");
            currentAmmo = 0;
        }
        else if (currentAmmo + amount > maxAmmo)
        {
            Debug.Log("Munici�n al m�ximo");
            currentAmmo = maxAmmo;
        }
        else
        {
            Debug.Log(amount + " de munici�n a�adida");
            currentAmmo += amount;
        }
    }
}
