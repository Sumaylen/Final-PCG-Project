using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    private bool savedRunData = false;
    private bool stageCheckpoint = false;

    // current variables
    private float currentHealth;
    private float maxHealth;
    private float meleeDamage;
    private float moveSpeed;
    private float attackSpeedBonus;
    private int level;
    private int currentXP;
    private int totalKills;



    // checkpoint stat variables
    private float savedCurrentHealth;
    private float savedMaxHealth;
    private float savedMeleeDamage;
    private float savedMoveSpeed;
    private float savedAttackSpeedBonus;
    private int savedLevel;
    private int savedCurrentXP;
    private int savedTotalKills;

    void Awake()
    {
        // ensures only the original instance exists to prevent overwrites
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // ensures game obejct isnt destroyed when scenes change
        DontDestroyOnLoad(gameObject);
    }

    public void ResetRunData()
    {
        savedRunData = false;
        stageCheckpoint = false;
        totalKills = 0;
        savedTotalKills = 0;
        KillCounter.SetKills(0);

    }

    // save all the current player stats
    public void SavePlayer(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerProgression progression = player.GetComponent<PlayerProgression>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        currentHealth = health.GetCurrentHealth();
        maxHealth = stats.GetMaxHealth();
        meleeDamage = stats.GetMeleeDamage();
        moveSpeed = stats.GetMoveSpeed();
        attackSpeedBonus = stats.GetAttackSpeedBonus();
        level = progression.GetLevel();
        currentXP = progression.GetCurrentXP();
        totalKills = KillCounter.GetKills();

        savedRunData = true;
    }

    // load saved player stats
    public void LoadPlayer(GameObject player)
    {
        if (savedRunData == false)
        {
            return;
        }

        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerProgression progression = player.GetComponent<PlayerProgression>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        stats.SetMeleeDamage(meleeDamage);
        stats.SetMoveSpeed(moveSpeed);
        stats.SetMaxHealth(maxHealth);
        stats.SetAttackSpeedBonus(attackSpeedBonus);

        progression.SetLevel(level);
        progression.SetCurrentXP(currentXP);

        health.SetCurrentHealth(currentHealth);
    }

    // creates a checkpoint that saves stats at the start of a new stage
    public void SaveStage()
    {
        if (savedRunData == false)
        {
            return;
        }

        savedCurrentHealth = currentHealth;
        savedMaxHealth = maxHealth;
        savedMeleeDamage = meleeDamage;
        savedMoveSpeed = moveSpeed;
        savedAttackSpeedBonus = attackSpeedBonus;
        savedLevel = level;
        savedCurrentXP = currentXP;
        savedTotalKills = totalKills;


        stageCheckpoint = true;
    }

    // loads checkpoint
    public void LoadStage()
    {
        if (stageCheckpoint == false)
        {
            return;
        }

        currentHealth = savedCurrentHealth;
        maxHealth = savedMaxHealth;
        meleeDamage = savedMeleeDamage;
        moveSpeed = savedMoveSpeed;
        attackSpeedBonus = savedAttackSpeedBonus;
        level = savedLevel;
        currentXP = savedCurrentXP;
        totalKills = savedTotalKills;
        KillCounter.SetKills(totalKills);


        savedRunData = true;
    }
}
