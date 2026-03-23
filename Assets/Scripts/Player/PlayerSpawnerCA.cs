using UnityEngine;

public class PlayerSpawnerCA : MonoBehaviour
{
    [SerializeField]
    private CADungeonGenerator CAGenerator;
    [SerializeField] private GameObject playerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {

        Vector3 spawnPos = CAGenerator.GetRandomFloorPosition();

        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        PlayerData.Instance.LoadPlayer(player);

    }
}
