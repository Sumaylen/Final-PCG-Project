using UnityEngine;
using TMPro;
using System;

public class LevelUpUi : MonoBehaviour
{
    public static bool LevelUpOpen = false;

    private PlayerProgression progression;
    private UpgradeSystem upgradeSystem;

    [SerializeField]
    private GameObject levelUpBanner;

    // stat name text for upgrades
    [SerializeField]
    private TextMeshProUGUI maxHealthUpgradeText;
    [SerializeField]
    private TextMeshProUGUI attackDamageUpgradeText;
    [SerializeField]
    private TextMeshProUGUI attackSpeedUpgradeText;
    [SerializeField]
    private TextMeshProUGUI movementSpeedUpgradeText;

    // stat values for upgrades
    [SerializeField]
    private TextMeshProUGUI maxHealthUpgradeValue;
    [SerializeField]
    private TextMeshProUGUI attackDamageUpgradeValue;
    [SerializeField]
    private TextMeshProUGUI attackSpeedUpgradeValue;
    [SerializeField]
    private TextMeshProUGUI movementSpeedUpgradeValue;
    [SerializeField]
    private TextMeshProUGUI levelValue;

    void Start()
    {
        levelUpBanner.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        progression = player.GetComponent<PlayerProgression>();
        upgradeSystem = player.GetComponent<UpgradeSystem>();

        progression.OnLevelChanged += HandleLevelChanged;
    }

    void OnDisable()
    {
        progression.OnLevelChanged -= HandleLevelChanged;
    }

    void HandleLevelChanged(int newLevel)
    {
        UpdateLevelText();
        ShowLevelUpBanner();
    }

    void ShowLevelUpBanner()
    {
        upgradeSystem.GetUpgradeValues();
        UpdateUpgradeValueTexts();
        levelUpBanner.SetActive(true);
        LevelUpOpen = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideLevelUpBanner()
    {
        levelUpBanner.SetActive(false);
        LevelUpOpen = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UpdateUpgradeValueTexts()
    {
        maxHealthUpgradeText.text = "Max Health";
        maxHealthUpgradeText.color = rarityColour(upgradeSystem.GetMaxHealthUpgrade().rarity);
        maxHealthUpgradeValue.text = "+" + upgradeSystem.GetMaxHealthUpgrade().currentUpgrade.ToString("0");

        attackDamageUpgradeText.text = "Attack Damage";
        attackDamageUpgradeText.color = rarityColour(upgradeSystem.GetAttackDamageUpgrade().rarity);
        attackDamageUpgradeValue.text = "+" + upgradeSystem.GetAttackDamageUpgrade().currentUpgrade.ToString("0");

        attackSpeedUpgradeText.text = "Attack Speed";
        attackSpeedUpgradeText.color = rarityColour(upgradeSystem.GetNextAttackSpeedUpgrade().rarity);
        float attackSpeedPercent = upgradeSystem.GetNextAttackSpeedUpgrade().currentUpgrade * 100f;
        attackSpeedUpgradeValue.text = "+" + attackSpeedPercent.ToString("0.#") + "%";

        movementSpeedUpgradeText.text = "Movement Speed";
        movementSpeedUpgradeText.color = rarityColour(upgradeSystem.GetMovementSpeedUpgrade().rarity);
        movementSpeedUpgradeValue.text = "+" + upgradeSystem.GetMovementSpeedUpgrade().currentUpgrade.ToString("0.#");
    }

    private Color rarityColour(UpgradeSystem.UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeSystem.UpgradeRarity.Common:
                return new Color(1f, 1f, 1f); // white
            case UpgradeSystem.UpgradeRarity.Rare:
                return new Color(0f, 0.439f, 0.867f); // blue
            case UpgradeSystem.UpgradeRarity.Epic:
                return new Color(0.639f, 0.208f, 0.933f); // purple
        }
        return new Color(1f, 1f, 1f);
    }

    void UpdateLevelText()
    {
        levelValue.text = progression.GetLevel().ToString();
    }

    public void OnUpgradeClick(int upgradeIndex)
    {
        //Debug.Log("Triggered upgrade index: " + upgradeIndex);
        upgradeSystem.ApplyUpgrade(upgradeIndex);
        HideLevelUpBanner();
    }

}
