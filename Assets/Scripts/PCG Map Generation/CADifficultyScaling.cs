using UnityEngine;

public class CADifficultyScaling : MonoBehaviour
{
    [SerializeField]
    private GameTimer gameTimer;
    [SerializeField]
    private EnemySpawnerCA enemySpawner;

    // trigger percentages for difficulty spikes
    [SerializeField, Range(0f, 100f)]
    private float firstTriggerPercent = 70f;
    [SerializeField, Range(0f, 100f)]
    private float secondTriggerPercent = 30f;

    // health percentage increase for each difficulty spike
    [SerializeField]
    private float firstHealthPercent = 20f;
    [SerializeField]
    private float secondHealthPercent = 25f;

    // damage percentage increase for each difficulty spike
    [SerializeField]
    private float firstDamagePercent = 20f;
    [SerializeField]
    private float secondDamagePercent = 25f;

    // min enemies alive increase for each difficulty spike
    [SerializeField]
    private float firstMinEnemiesAlivePercent = 20f;
    [SerializeField]
    private float secondMinEnemiesAlivePercent = 25f;

    // respawn delay reduction for each difficulty spike
    [SerializeField]
    private float firstRespawnReductionPercent = 20f;
    [SerializeField]
    private float secondRespawnReductionPercent = 25f;

    // elite spawn chance for each difficulty spike
    [SerializeField]
    private float startEliteChance = 0f;
    [SerializeField]
    private float firstEliteChance = 0.20f;
    [SerializeField]
    private float secondEliteChance = 0.30f;

    // bools so that the difficulty spikes only trigger once
    private bool firstTriggered = false;
    private bool secondTriggered = false;

    void Start()
    {
        enemySpawner.SetEliteSpawnChance(startEliteChance);
    }

    // triggers difficulty spike based on time remaining
    void Update()
    {
        float timeRemainingPercent = gameTimer.GetTimeRemainingPercent();

        if (firstTriggered == false && timeRemainingPercent <= firstTriggerPercent)
        {
            firstTriggered = true;
            IncreaseEnemyHealth(firstHealthPercent);
            IncreaseEnemyMeleeDamage(firstDamagePercent);
            enemySpawner.IncreaseMinEnemiesAlive(firstMinEnemiesAlivePercent);
            enemySpawner.ReduceRespawnDelay(firstRespawnReductionPercent);
            enemySpawner.SetEliteSpawnChance(firstEliteChance);
        }

        if (secondTriggered == false && timeRemainingPercent <= secondTriggerPercent)
        {
            secondTriggered = true;
            IncreaseEnemyHealth(secondHealthPercent);
            IncreaseEnemyMeleeDamage(secondDamagePercent);
            enemySpawner.IncreaseMinEnemiesAlive(secondMinEnemiesAlivePercent);
            enemySpawner.ReduceRespawnDelay(secondRespawnReductionPercent);
            enemySpawner.SetEliteSpawnChance(secondEliteChance);
        }
    }

    // increases max health of enemies
    void IncreaseEnemyHealth(float percent)
    {
        // find all enmies in the sscene
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].IncreaseMaxHealth(percent);
        }
    }

    // increases melee damage of enmies
    void IncreaseEnemyMeleeDamage(float percent)
    {
        // find all enmies in the sscene
        EnemyCombatMelee[] enemies = FindObjectsByType<EnemyCombatMelee>(FindObjectsSortMode.None);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].IncreaseMeleeDamage(percent);
        }
    }
}
