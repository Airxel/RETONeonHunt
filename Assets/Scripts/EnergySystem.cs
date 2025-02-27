using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    public bool canShoot = true;
    [SerializeField]
    private float totalEnergy = 100f;
    [SerializeField]
    public float currentEnergy;
    [SerializeField]
    public float energyPerShoot = 33f;
    [SerializeField]
    private float energyPerSecond = 3f;
    [SerializeField]
    private float energyCooldown = 10f;
    [SerializeField]
    public float currentEnergyCooldown;
    [SerializeField]
    public bool energyRechargePowerUpActive;
    [SerializeField]
    private float energyRechargePowerUpDuration = 30f;
    [SerializeField]
    private float currentEnergyRechargePowerUpDuration;

    //Singleton
    public static EnergySystem instance;

    private void Awake()
    {
        if (EnergySystem.instance == null)
        {
            EnergySystem.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentEnergy = totalEnergy;
        currentEnergyCooldown = energyCooldown;
        canShoot = true;
        energyRechargePowerUpActive = false;
        currentEnergyRechargePowerUpDuration = energyRechargePowerUpDuration;
        UIManager.instance.energyNumber.text = currentEnergy.ToString("F000") + " % ";
    }


    private void Update()
    {
        CanShoot();
        EnergyPerSecond();
    }

    public void CanShoot()
    {
        if (currentEnergy >= energyPerShoot)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }

    public void EnergyRecharge(float energyGain)
    {
        currentEnergy += energyGain; 
    }

    public void EnergyPerSecond()
    {
        if (energyRechargePowerUpActive == true)
        {
            currentEnergy = totalEnergy;
            EnergyRecharPowerUpDuration();
        }
        else if (energyRechargePowerUpActive == false)
        {
            if (playerController.rechargeReady == true)
            {
                currentEnergyCooldown -= Time.deltaTime;

                if (currentEnergyCooldown <= 0 && currentEnergy < 100f)
                {
                    currentEnergy += energyPerSecond * Time.deltaTime;
                }
            }
            else if (playerController.rechargeReady == false)
            {
                currentEnergyCooldown = 10f;
            }
        }

        UIManager.instance.energyNumber.text = currentEnergy.ToString("F000") + " % ";
    }

    private void EnergyRecharPowerUpDuration()
    {
        if (energyRechargePowerUpActive == true)
        {
            currentEnergyRechargePowerUpDuration -= Time.deltaTime;

            if (currentEnergyRechargePowerUpDuration <= 0f)
            {
                energyRechargePowerUpActive = false;
                currentEnergyRechargePowerUpDuration = energyRechargePowerUpDuration;
            }
        }
    }
}
