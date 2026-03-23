using UnityEngine;
using System;

public class PlayerProgression : MonoBehaviour
{

    [SerializeField]
    private int level = 1;
    [SerializeField]
    private int currentXP = 0;
    // xp required for first lvl up
    [SerializeField]
    private int nextXP = 100;
    // multiplier for how much more xp is needed for each lvl
    [SerializeField]
    private float xpMultiplier = 1.25f;

    // events that other scripts can subscribe to for level and xp changes
    public event Action<int> OnLevelChanged;
    // currentXP, xpToNext
    public event Action<int, int> OnXPChanged;

    public void AddXP(int amount)
    {

        currentXP += amount;

        // runs if have enough exp for the next lvl
        while (currentXP >= CalculateXP(level))
        {
            int needed = CalculateXP(level);
            currentXP -= needed;
            level += 1;

            Debug.Log("Level Up , New level = " + level);
            //ApplyLevelUpBonus();
            OnLevelChanged(level);
        }
        int xpToNext = CalculateXP(level);
        //Debug.Log("XP to next level = " + xpToNext);
        OnXPChanged(currentXP, xpToNext);
        // pass this player object to PlayerData so its xp can be saved
        PlayerData.Instance.SavePlayer(gameObject);
    }

    // returns the xp needed for the next level based on the current level and multiplier
    private int CalculateXP(int lvl)
    {
        // exponetial xp scaling
        float value = nextXP * Mathf.Pow(xpMultiplier, lvl - 1);
        return Mathf.CeilToInt(value); // round up to next int
    }

    // getter functions
    public int GetLevel()
    {
        return level;
    }

    public int GetCurrentXP()
    {
        return currentXP;
    }

    public int GetXPToNext()
    {
        return CalculateXP(level);
    }

    // set functions
    public void SetLevel(int value)
    {
        level = value;
    }

    public void SetCurrentXP(int value)
    {
        currentXP = value;
    }


}
