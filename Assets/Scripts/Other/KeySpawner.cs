using UnityEngine;
using System.Collections.Generic;

public class KeySpawner : MonoBehaviour
{
    [SerializeField]
    private BSPDungeonGenerator dungeonGenerator;
    [SerializeField]
    private GameObject keyPrefab;
    [SerializeField]
    private KeyController keyController;
    [SerializeField]
    private int keysToSpawn = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnKeys();
    }

    void SpawnKeys()
    {
        BSPNode root = dungeonGenerator.GetRoot();
        RectInt spawnRoom = dungeonGenerator.GetSpawnRoom();

        List<RectInt> rooms = BSPHelper.GetAllRooms(root);

        // Remove spawn room
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if (rooms[i] == spawnRoom)
            {
                rooms.RemoveAt(i);
            }
        }

        // reduce amount of keys if not enough room
        if (rooms.Count < keysToSpawn)
        {
            keysToSpawn = rooms.Count;
        }

        // tel lthe controller how many keys have spawned
        keyController.KeysSpawned(keysToSpawn);

        // spawn keys in random rooms
        for (int i = 0; i < keysToSpawn; i++)
        {
            int index = Random.Range(0, rooms.Count);
            RectInt room = rooms[index];
            rooms.RemoveAt(index); // prevents having 2 keys in the same room

            Vector2Int center = BSPHelper.GetRoomCenter(room);
            Vector3 pos = new Vector3(center.x, 0.5f, center.y);

            GameObject keyObject = Instantiate(keyPrefab, pos, Quaternion.identity);
            // assign the controller to KeyPickup
            KeyPickup pickup = keyObject.GetComponent<KeyPickup>();
            pickup.SetKeyController(keyController);
        }
    }
}
