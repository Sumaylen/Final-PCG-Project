using UnityEngine;

public class PortalController : MonoBehaviour
{
    [SerializeField]
    private BSPDungeonGenerator dungeonGenerator;


    [SerializeField]
    private GameObject portalInactivePrefab;
    [SerializeField]
    private GameObject portalActivePrefab;


    private GameObject portalInstance;
    private bool activated = false;

    void Start()
    {
        SpawnInactivePortal();
    }

    void Update()
    {
        if (activated == true)
        {
            return;
        }

        bool keysReady = KeyController.HasEnoughKeys();
        bool killsReady = KillCounter.HasEnoughKills();

        if (keysReady == true && killsReady == true)
        {
            ActivatePortal();
        }
    }

    void SpawnInactivePortal()
    {
        RectInt spawnRoom = dungeonGenerator.GetSpawnRoom();
        Vector2Int center = BSPHelper.GetRoomCenter(spawnRoom);

        Vector3 pos = new Vector3(center.x + 5f, 0f, center.y + 0.5f);
        portalInstance = Instantiate(portalInactivePrefab, pos, Quaternion.identity);

        activated = false;
    }

    void ActivatePortal()
    {
        activated = true;

        Vector3 pos = portalInstance.transform.position;
        Quaternion rot = portalInstance.transform.rotation;

        Destroy(portalInstance);

        portalInstance = Instantiate(portalActivePrefab, pos, Quaternion.identity);
        
    }
}
