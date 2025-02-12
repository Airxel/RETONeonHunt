using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    public int maxHealth = 5;
    [SerializeField]
    public int currentHealth = 2;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        UpdateAnimator();
    }

    public void ModifyHealth(int amount)
    {
        if (currentHealth + amount <= 0)
        {
            Debug.Log("Muerto");
            currentHealth = 0;
        }
        else if (currentHealth + amount > maxHealth)
        {
            Debug.Log("Vida al máximo");
            currentHealth = maxHealth;
        }
        else
        {
            Debug.Log("Recupera " + amount + " de vida");
            currentHealth += amount;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetInteger("Health", currentHealth);
    }
}
