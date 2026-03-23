using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    private static KillCounter instance;
    private static int kills;
    private static int required;
    [SerializeField]
    private int killsRequired = 20;

    [SerializeField]
    private TextMeshProUGUI killCounterText;

    void Awake()
    {
        instance = this;
        kills = 0;
        required = killsRequired;
        UpdateUI();
    }
    public static void AddKill(int amount)
    {
        kills += amount;
        if (instance != null)
        {
            instance.UpdateUI();
        }
    }

    public static bool HasEnoughKills()
    {
        return kills >= required;
    }

    private void UpdateUI()
    {
        killCounterText.text = $"Enemies Defeated: {kills}/{required}";
    }

    public static int GetKills()
    {
        return kills;
    }

    public static void SetKills(int value)
    {
        kills = value;
    }



}
