using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cuando es : significa que este script es hijo de otro, en ese caso de MonoBehaviour y IPowerUp
public class PickUpLife : MonoBehaviour, IPowerUp
{
    [SerializeField]
    int healthAmount = 1;

    public void PickUpPowerUp(GameObject other)
    {
        if (other.TryGetComponent<HealthSystem>(out HealthSystem health))
        {
            if (health.currentHealth < health.maxHealth)
            {
                health.ModifyHealth(healthAmount);
                Debug.Log("Añadir " + healthAmount + " de salud");

                Destroy(this.gameObject);
            }
            else
            {
                return;
            }
        }

        //HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        
        //if (healthSystem != null)
        //{
        //  healthSystem.ModifyHealth(healthAmount);
        //    Debug.Log("Añadir " + healthAmount + " salud");
        //}
    }
}
