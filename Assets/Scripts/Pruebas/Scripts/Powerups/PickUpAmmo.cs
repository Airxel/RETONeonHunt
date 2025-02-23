using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAmmo : MonoBehaviour, IPowerUp
{
    [SerializeField]
    int ammoAmount = 1;
    public void PickUpPowerUp(GameObject other)
    {
        if (other.TryGetComponent<AmmoSystem>(out AmmoSystem ammo))
        {
            if (ammo.currentAmmo < ammo.maxAmmo)
            {
                ammo.ModifyAmmo(ammoAmount);
                Debug.Log("Añadir " + ammoAmount + " de munición");
            }
            else
            {
                return;
            }
        }
    }
}
