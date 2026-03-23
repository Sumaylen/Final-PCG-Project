using System;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public enum UpgradeRarity
    {
        Common,
        Rare,
        Epic
    }

    public class UpgradeOption
    {
        public float currentUpgrade;
        public float baseAttackSpeedGain;
        public int attackSpeedSteps;
        public UpgradeRarity rarity;
    }

    private PlayerStats playerStats;

    // base upgrade amounts
    [SerializeField]
    private float baseMaxHealthUpgrade = 20f;
    [SerializeField]
    private float baseAttackDamageUpgrade = 5f;
    [SerializeField]
    private float baseMovementSpeedUpgrade = 1f;
    [SerializeField]
    private float baseAttackSpeedGain = 0.10f;

    // controls variance on base uupgrade amounts
    [SerializeField]
    private float upgradeVariancePercent = 0.15f;

    // rarity chances
    [SerializeField]
    private float commonChance = 70f;
    [SerializeField]
    private float rareChance = 25f;
    [SerializeField]
    private float epicChance = 5f;

    // rarity stat multipliers
    [SerializeField]
    private float commonMultiplier = 1f;
    [SerializeField]
    private float rareMultiplier = 1.5f;
    [SerializeField]
    private float epicMultiplier = 2f;
    // for atatck speed, run the base upgrade multiple times based on rarity
    [SerializeField]
    private int commonAttackSpeedSteps = 1;
    [SerializeField]
    private int rareAttackSpeedSteps = 2;
    [SerializeField]
    private int epicAttackSpeedSteps = 3;

    public event Action OnUpgradeApplied;


    // upgrade options to store the current upgrade values for each stat
    private UpgradeOption maxHealthUpgrade = new UpgradeOption();
    private UpgradeOption attackDamageUpgrade = new UpgradeOption();
    private UpgradeOption attackSpeedUpgrade = new UpgradeOption();
    private UpgradeOption movementSpeedUpgrade = new UpgradeOption();


    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // calls functions to get the upgrade amounts
    public void GetUpgradeValues()
    {
        maxHealthUpgrade.currentUpgrade = HealthUpgrade();

        attackDamageUpgrade.currentUpgrade = AttackDamageUpgrade();

        attackSpeedUpgrade.currentUpgrade = AttackSpeedUpgrade();

        movementSpeedUpgrade.currentUpgrade = MovementSpeedUpgrade();
    }

    // applies the stat increase
    public void ApplyUpgrade(int upgradeIndex)
    {
        switch (upgradeIndex)
        {
            case 0:
                playerStats.AddMaxHealth(maxHealthUpgrade.currentUpgrade);
                break;
            case 1:
                playerStats.AddMeleeDamage(attackDamageUpgrade.currentUpgrade);
                break;
            case 2:
                playerStats.IncreaseAttackSpeed(attackSpeedUpgrade.attackSpeedSteps, attackSpeedUpgrade.baseAttackSpeedGain);
                break;
            case 3:
                playerStats.AddMoveSpeed(movementSpeedUpgrade.currentUpgrade);
                break;
        }

        Debug.Log("Upgrade " + upgradeIndex);
        OnUpgradeApplied();
    }

    // calculate rarity chances
    private UpgradeRarity RollRarity()
    {
        float totalChance = commonChance + rareChance + epicChance;

        float roll = UnityEngine.Random.Range(0f, totalChance);
        if (roll < commonChance)
        {
            return UpgradeRarity.Common;
        }

        else if (roll < rareChance + commonChance)
        {
            return UpgradeRarity.Rare;
        }

        else
        {
            return UpgradeRarity.Epic;
        }
    }

    // check what the multiplier should be based on rarity
    private float GetRarityMultiplier(UpgradeRarity rarity)
    {
        float multiplier = commonMultiplier;
        switch (rarity)
        {
            case UpgradeRarity.Common:
                multiplier = commonMultiplier;
                break;
            case UpgradeRarity.Rare:
                multiplier = rareMultiplier;
                break;
            case UpgradeRarity.Epic:
                multiplier = epicMultiplier;
                break;
        }

        return multiplier;
    }

    // check how many times the attack speed upgrade should run based on rarity
    private int GetAttackSpeedSteps(UpgradeRarity rarity)
    {
        int steps = commonAttackSpeedSteps;
        switch (rarity)
        {
            case UpgradeRarity.Common:
                steps = commonAttackSpeedSteps;
                break;
            case UpgradeRarity.Rare:
                steps = rareAttackSpeedSteps;
                break;
            case UpgradeRarity.Epic:
                steps = epicAttackSpeedSteps;
                break;
        }

        return steps;
    }

    // functions to calculate the upgrade amounts based on rarity
    // also adds some variance to the base upgrade amounts
    private float HealthUpgrade()
    {
        UpgradeRarity rarity = RollRarity();
        maxHealthUpgrade.rarity = rarity;
        float multiplier = GetRarityMultiplier(rarity);
        float variance = UnityEngine.Random.Range(-upgradeVariancePercent, upgradeVariancePercent) * baseMaxHealthUpgrade;
        float upgradeAmount = baseMaxHealthUpgrade + variance;
        return upgradeAmount * multiplier;
    }
    private float AttackDamageUpgrade()
    {
        UpgradeRarity rarity = RollRarity();
        attackDamageUpgrade.rarity = rarity;
        float multiplier = GetRarityMultiplier(rarity);
        float variance = UnityEngine.Random.Range(-upgradeVariancePercent, upgradeVariancePercent) * baseAttackDamageUpgrade;
        float upgradeAmount = baseAttackDamageUpgrade + variance;
        return upgradeAmount * multiplier;
    }
    private float AttackSpeedUpgrade()
    {
        UpgradeRarity rarity = RollRarity();
        attackSpeedUpgrade.rarity = rarity;
        attackSpeedUpgrade.attackSpeedSteps = GetAttackSpeedSteps(rarity);
        float variance = UnityEngine.Random.Range(-upgradeVariancePercent, upgradeVariancePercent) * baseAttackSpeedGain;
        attackSpeedUpgrade.baseAttackSpeedGain = baseAttackSpeedGain + variance;
        return playerStats.GetAttackSpeedIncrease(attackSpeedUpgrade.attackSpeedSteps, attackSpeedUpgrade.baseAttackSpeedGain);
    }
    private float MovementSpeedUpgrade()
    {
        UpgradeRarity rarity = RollRarity();
        movementSpeedUpgrade.rarity = rarity;
        float multiplier = GetRarityMultiplier(rarity);
        float variance = UnityEngine.Random.Range(-upgradeVariancePercent, upgradeVariancePercent) * baseMovementSpeedUpgrade;
        float upgradeAmount = baseMovementSpeedUpgrade + variance;
        return upgradeAmount * multiplier;
    }

    // getter meothods
    public UpgradeOption GetMaxHealthUpgrade()
    {
        return maxHealthUpgrade;
    }
    public UpgradeOption GetAttackDamageUpgrade()
    {
        return attackDamageUpgrade;
    }
    public UpgradeOption GetNextAttackSpeedUpgrade()
    {
        return attackSpeedUpgrade;
    }

    public UpgradeOption GetMovementSpeedUpgrade()
    {
        return movementSpeedUpgrade;
    }
}
