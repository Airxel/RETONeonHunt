using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpAbility : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Si busca que tenga IPowerUp, si lo encuentra, lo guarda y hace la función, el out te evida poner !=null
        if (other.TryGetComponent<IPowerUp>(out IPowerUp pickUpPowerup))
        {
            pickUpPowerup.PickUpPowerUp(this.gameObject);
        }
    }
}
