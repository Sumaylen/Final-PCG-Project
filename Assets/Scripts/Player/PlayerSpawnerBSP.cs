using UnityEngine;

public class PlayerSpawnerBSP : MonoBehaviour
{

    [SerializeField]
    private BSPDungeonGenerator dungeonGenerator;
    [SerializeField]
    private GameObject playerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPlayer();
    }
    
    // spawn player at the center of a randomyl assigned spawn room
    void SpawnPlayer()
    {

        RectInt spawnRoom = dungeonGenerator.GetRandomRoom();
        dungeonGenerator.SetSpawnRoom(spawnRoom);
        Vector2Int center = GetRoomCenter(spawnRoom);

        Vector3 spawnPos = new Vector3(center.x + 0.5f, 0f, center.y + 0.5f);

        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        PlayerData.Instance.LoadPlayer(player);

    }

    Vector2Int GetRoomCenter(RectInt room)
    {
        return new Vector2Int(
            room.x + room.width / 2,
            room.y + room.height / 2
        );
    }
}