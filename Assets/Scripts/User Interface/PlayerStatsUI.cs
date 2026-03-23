using UnityEngine;
using TMPro;


public class PlayerStatsUI : MonoBehaviour
{
    private PlayerStats playerStats;
    private UpgradeSystem upgradeSystem;

    [SerializeField]
    private TextMeshProUGUI maxHealth;
    [SerializeField]
    private TextMeshProUGUI meleeDamage;
    [SerializeField]
    private TextMeshProUGUI attackSpeed;
    [SerializeField]
    private TextMeshProUGUI moveSpeed;

    [SerializeField]
    private UnityEngine.Color statsTextColor;

    private bool isDamageBuffActive = false;
    private bool isMoveSpeedBuffActive = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        upgradeSystem = player.GetComponent<UpgradeSystem>();

        upgradeSystem.OnUpgradeApplied += UpdateStatsText;
        playerStats.OnStatChanged += UpdateBuffText;
        playerStats.OnStatReverted += RevertStatsText;
        UpdateStatsText();
    }

    void OnDisable()
    {
        upgradeSystem.OnUpgradeApplied -= UpdateStatsText;
        playerStats.OnStatChanged -= UpdateBuffText;
        playerStats.OnStatReverted -= RevertStatsText;
    }


    void UpdateStatsText()
    {
        if (isDamageBuffActive == false)
        {
            meleeDamage.color = statsTextColor;

        }

        if (isMoveSpeedBuffActive == false)
        {
            moveSpeed.color = statsTextColor;
        }

        maxHealth.color = statsTextColor;
        attackSpeed.color = statsTextColor;
        maxHealth.text = playerStats.GetMaxHealth().ToString("0");
        meleeDamage.text = playerStats.GetMeleeDamage().ToString("0");
        attackSpeed.text = playerStats.GetAttackSpeedMultiplier().ToString("0.00");
        moveSpeed.text = playerStats.GetMoveSpeed().ToString("0.0");
    }

    void RevertStatsText(string buffType)
    {
        if (buffType == "damageBuff")
        {
            isDamageBuffActive = false;
        }
        if (buffType == "moveSpeedBuff")
        {
            isMoveSpeedBuffActive = false;
        }
        UpdateStatsText();
    }

    void UpdateBuffText(string buffType)
    {
        if (buffType == "damageBuff")
        {
            isDamageBuffActive = true;
            meleeDamage.color = UnityEngine.Color.red;
            meleeDamage.text = playerStats.GetMeleeDamage().ToString("0");
        }

        if (buffType == "moveSpeedBuff")
        {
            isMoveSpeedBuffActive = true;
            moveSpeed.color = UnityEngine.Color.red;
            moveSpeed.text = playerStats.GetMoveSpeed().ToString("0.0");
        }
    }


}
