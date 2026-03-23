using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerBSP : MonoBehaviour
{
    [SerializeField]
    private BSPDungeonGenerator dungeonGenerator;

    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject eliteEnemyPrefab;
    [SerializeField]
    private float eliteSpawnChance = 0f;

    [SerializeField]
    private int enemiesPerRoom = 6;

    [SerializeField]
    private float respawnDelay = 3f;

    [SerializeField]
    private int spawnPadding = 1;

    [SerializeField]
    private float spawnHeight = 0f;

    //Prevent spawning on top of player/enemies 
    [SerializeField]
    private float spawnCheckRadius = 0.4f;

    [SerializeField]
    private LayerMask blockingLayers;


    private List<RoomState> rooms = new List<RoomState>();


    class RoomState
    {
        public RectInt room;
        public List<GameObject> enemies = new List<GameObject>();
        public float nextSpawnTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupRooms();
        SpawnInitialEnemies();
    }

    // Update is called once per frame
    // Thus update tries to ensure minimum number of enemies in each room
    void Update()
    {
        float now = Time.time;

        for (int i = 0; i < rooms.Count; i++)
        {
            RoomState state = rooms[i];

            // Clean up dead enemies
            CleanupEnemies(state);

            // If room already has enough enemies, do nothing
            if (state.enemies.Count >= enemiesPerRoom)
            {
                continue;
            }

            // Respawn delay
            if (now < state.nextSpawnTime)
            {
                continue;
            }

            // Try to spawn an enemy in this room if possible
            if (SpawnInRoom(state) == true)
            {
                state.nextSpawnTime = now + respawnDelay;
            }
            else
            {
                // If we fail to spawn an enemy, try again later
                state.nextSpawnTime = now + respawnDelay;
            }
        }
    }

    void SetupRooms()
    {
        // needed to clear the room list on new map gnerations
        rooms.Clear();

        BSPNode root = dungeonGenerator.GetRoot();
        RectInt spawnRoom = dungeonGenerator.GetSpawnRoom();

        List<RectInt> allRooms = BSPHelper.GetAllRooms(root);

        for (int i = 0; i < allRooms.Count; i++)
        {
            RectInt room = allRooms[i];

            // Skip player spawn room
            if (room == spawnRoom)
            {
                continue;
            }

            // add rooms to our  class list
            RoomState state = new RoomState();
            state.room = room;
            state.nextSpawnTime = Time.time;
            rooms.Add(state);
        }
    }

    void SpawnInitialEnemies()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            RoomState state = rooms[i];

            for (int j = 0; j < enemiesPerRoom; j++)
            {
                SpawnInRoom(state);
            }

            // sets next spawn time after initial spawn
            state.nextSpawnTime = Time.time + respawnDelay;
        }
    }

    bool SpawnInRoom(RoomState state)
    {
        RectInt room = state.room;

        //prevents spawning too close to walls
        int minX = room.x + spawnPadding;
        int maxX = room.x + room.width - spawnPadding - 1;

        int minY = room.y + spawnPadding;
        int maxY = room.y + room.height - spawnPadding - 1;

        if (minX > maxX || minY > maxY)
        {
            return false;
        }

        // Get random spawn points within the room
        for (int spawnPoints = 0; spawnPoints < 12; spawnPoints++)
        {
            int x = Random.Range(minX, maxX + 1);
            int y = Random.Range(minY, maxY + 1);

            Vector3 pos = new Vector3(x + 0.5f, spawnHeight, y + 0.5f);

            // skip if collision with a blocked layer
            Collider[] blockers = Physics.OverlapSphere(pos, spawnCheckRadius, blockingLayers, QueryTriggerInteraction.Collide);
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

            GameObject enemy = Instantiate(prefabToSpawn, pos, Quaternion.identity);
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            ai.InRoom(state.room);
            state.enemies.Add(enemy);
            return true;
        }

        return false;
    }

    void CleanupEnemies(RoomState state)
    {
        // Remove dead enemies from the list
        for (int i = state.enemies.Count - 1; i >= 0; i--)
        {
            if (state.enemies[i] == null)
            {
                state.enemies.RemoveAt(i);
            }
        }
    }

    // function to increase amount of enmies per room by a percent
    public void IncreaseEnemiesPerRoom(float percent)
    {
        float increaseAmount = enemiesPerRoom * (percent / 100f);
        enemiesPerRoom += Mathf.CeilToInt(increaseAmount); // round up
    }

    // increase spawn rate
    public void ReduceRespawnDelay(float percent)
    {
        respawnDelay -= respawnDelay * (percent / 100f);
    }
    public float GetEliteSpawnChance()
    {
        return eliteSpawnChance;
    }

    public void SetEliteSpawnChance(float chance)
    {
        eliteSpawnChance = chance;
    }
}
