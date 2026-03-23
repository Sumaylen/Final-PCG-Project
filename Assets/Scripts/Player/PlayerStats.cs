using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [SerializeField]
    private float meleeDamage = 20f;
    [SerializeField]
    private float moveSpeed = 6f;
    [SerializeField]
    private float maxHealth = 100f;


    [SerializeField]
    private float attackSpeedBonus = 0f;

    public event Action<float> OnHealthChanged;
    public event Action<string> OnStatChanged;
    public event Action<string> OnStatReverted;

    private Coroutine damageBuffCoroutine;
    private float damageBuffAmount;
    private bool damageBuffActive = false;

    private Coroutine moveSpeedBuffCoroutine;
    private float moveSpeedBuffAmount;
    private bool moveSpeedBuffActive = false;



    public void AddMeleeDamage(float amount)
    {
        meleeDamage += amount;
        Debug.Log("Melee damage is now " + meleeDamage);
        // pass this player object to PlayerData so its stats can be saved
        PlayerData.Instance.SavePlayer(gameObject);
    }

    public void AddMoveSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log("Move speed is now " + moveSpeed);
        // pass this player object to PlayerData so its stats can be saved
        PlayerData.Instance.SavePlayer(gameObject);
    }

    public void AddMaxHealth(float amount)
    {
        maxHealth += amount;
        OnHealthChanged(amount);
        Debug.Log("Max health is now " + maxHealth);
        // pass this player object to PlayerData so its stats can be saved
        PlayerData.Instance.SavePlayer(gameObject);
    }

    public void IncreaseAttackSpeed(int times, float baseAttackSpeedGain)
    {
        attackSpeedBonus += GetAttackSpeedIncrease(times, baseAttackSpeedGain);
        Debug.Log("Attack speed multiplier is now " + GetAttackSpeedMultiplier());
        // pass this player object to PlayerData so its stats can be saved
        PlayerData.Instance.SavePlayer(gameObject);
    }

    public void ApplyDamageBuff(float damageMultiplier, float duration)
    {
        // if there is no active buff apply damage increase
        if (damageBuffActive == false)
        {
            float damageIncrease = (meleeDamage * damageMultiplier) - meleeDamage;
            damageBuffAmount = damageIncrease;
            AddMeleeDamage(damageBuffAmount);
            OnStatChanged("damageBuff"); // event invoke to update the ui
            damageBuffActive = true;
        }
        else
        {
            // stop the coroutine to reset the timer
            StopCoroutine(damageBuffCoroutine);
        }
        // start coreroutine
        damageBuffCoroutine = StartCoroutine(DamageBuffTimer(duration));
    }

    // coreroutine waits for specified duration before rmeoving the buff 
    private IEnumerator DamageBuffTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        AddMeleeDamage(-damageBuffAmount);
        damageBuffAmount = 0f;
        damageBuffActive = false;
        damageBuffCoroutine = null;
        OnStatReverted("damageBuff");
    }

    public void ApplyMoveSpeedBuff(float speedMultiplier, float duration)
    {
        if (moveSpeedBuffActive == false)
        {
            moveSpeedBuffAmount = (moveSpeed * speedMultiplier) - moveSpeed;
            AddMoveSpeed(moveSpeedBuffAmount);
            OnStatChanged("moveSpeedBuff");
            moveSpeedBuffActive = true;
        }
        else
        {
            StopCoroutine(moveSpeedBuffCoroutine);
        }
        moveSpeedBuffCoroutine = StartCoroutine(MoveSpeedBuffTimer(duration));
    }

    private IEnumerator MoveSpeedBuffTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        AddMoveSpeed(-moveSpeedBuffAmount);
        moveSpeedBuffAmount = 0f;
        moveSpeedBuffActive = false;
        moveSpeedBuffCoroutine = null;
        OnStatReverted("moveSpeedBuff");
    }


    public float GetAttackSpeedIncrease(int times, float baseAttackSpeedGain)
    {
        // temp variables
        float currentAttackSpeedBonus = attackSpeedBonus;
        float attackSpeedUpgradeAmount = 0f;
        // diminishing returns, the more attack speed bonus you take the less effective each one is
        for (int i = 0; i < times; i++)
        {
            float increase = baseAttackSpeedGain / (1f + currentAttackSpeedBonus);
            currentAttackSpeedBonus += increase;
            attackSpeedUpgradeAmount += increase;
        }
        return attackSpeedUpgradeAmount;
    }


    // getter methods
    public float GetMeleeDamage()
    {
        return meleeDamage;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetAttackSpeedMultiplier()
    {
        return 1f + attackSpeedBonus;
    }

    public float GetNextAttackSpeedIncrease(float baseAttackSpeedGain)
    {
        return baseAttackSpeedGain / (1f + attackSpeedBonus);
    }

    public float GetAttackSpeedBonus()
    {
        return attackSpeedBonus;
    }

    // Set methods
    public void SetMeleeDamage(float value)
    {
        meleeDamage = value;
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
    }

    public void SetAttackSpeedBonus(float value)
    {
        attackSpeedBonus = value;
    }

}
