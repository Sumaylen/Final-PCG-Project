using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawnerCA : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject eliteEnemyPrefab;
    [SerializeField]
    private float eliteSpawnChance = 0f;

    [SerializeField]
    private int minEnemiesAlive = 12;
    [SerializeField]
    private float respawnDelay = 1.5f;

    [SerializeField]
    private float minSpawnRadius = 8f;
    [SerializeField]
    private float maxSpawnRadius = 16f;

    [SerializeField]
    private float spawnHeight = 0f;

    //Prevent spawning on top of player/enemies 
    [SerializeField]
    private float spawnCheckRadius = 0.5f;
    [SerializeField]
    private LayerMask blockingLayers;

    private List<GameObject> enemies = new List<GameObject>();
    private float nextSpawnTime;
    GameObject playerObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        playerObj = GameObject.FindWithTag("Player");
        nextSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Clean up dead enemies
        CleanupEnemies();

        // If enough enemies are alive do nothhing
        if (enemies.Count >= minEnemiesAlive)
        {
            return;
        }

        // if not enough time has passed since last spawn do nothing
        if (Time.time < nextSpawnTime)
        {
            return;
        }

        // spawn a new enemy and set the next spawn time
        if (SpawnEnemy() == true)
        {
            nextSpawnTime = Time.time + respawnDelay;
        }
    }

    bool SpawnEnemy()
    {
        Vector3 playerPos = playerObj.transform.position;

        //try 12 random positions
        for (int i = 0; i < 12; i++)
        {
            // pick a random angle and distance from the player within the spawn radius
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(minSpawnRadius, maxSpawnRadius);

            // convert from polar coordinates to cartesian coordinates to get spawn position
            Vector3 spawnPos = new Vector3(
                playerPos.x + Mathf.Cos(angle) * dist,
                spawnHeight,
                playerPos.z + Mathf.Sin(angle) * dist
            );

            // check if spawn position is on the navmesh, skip loop and try another position if it is not
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas) == false)
            {
                continue;
            }

            // skip loop and try another position if there is no valid path from the spawn position to the player
            if (HasPathToPlayer(hit.position) == false)
                continue;

            // skip if collision with a blocked layer
            Collider[] blockers = Physics.OverlapSphere(hit.position, spawnCheckRadius, blockingLayers, QueryTriggerInteraction.Collide);

            if (blockers.Length > 0)
            {
                continue;
            }

            // spawns elite enemy based on elite spawn chance otherwise spawns normal enemy
            GameObject prefabToSpawn = enemyPrefab;
            if (Random.value < eliteSpawnChance)
            {
                prefabToSpawn = eliteEnemyPrefab;
            }

            GameObject enemy = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);
            enemies.Add(enemy);
            return true;
        }

        return false;
    }

    // returns true if there is a valid path from the spawn position to the player position
    bool HasPathToPlayer(Vector3 spawnPos)
    {
        NavMeshPath path = new NavMeshPath();
        // calculate a path from the spawn position to the player position and stores it to path
        bool pathCheck = NavMesh.CalculatePath(spawnPos, playerObj.transform.position, NavMesh.AllAreas, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    void CleanupEnemies()
    {
        // Remove dead enemies from the list
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    // function to increase minimum alive enemies by a percent
    public void IncreaseMinEnemiesAlive(float percent)
    {
        float increaseAmount = minEnemiesAlive * (percent / 100f);
        minEnemiesAlive += Mathf.CeilToInt(increaseAmount); // round up
    }

    // increase spawn rate
    public void ReduceRespawnDelay(float percent)
    {
        respawnDelay -= respawnDelay * (percent / 100f);
    }

    public void SetEliteSpawnChance(float chance)
    {
        eliteSpawnChance = chance;
    }
}
