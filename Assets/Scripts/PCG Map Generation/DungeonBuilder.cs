using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class DungeonBuilder : MonoBehaviour
{
    [SerializeField]
    private BSPDungeonGenerator generator;
    [SerializeField]
    private GameObject floorPrefab;
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private GameObject ceilingPrefab;
    [SerializeField]
    private float wallHeight = 10f;
    [SerializeField]
    private NavMeshSurface surface;
    [SerializeField]
    private float lightHeight = 2.5f;
    [SerializeField]
    private Color lightColor = new Color(1f, 0.92f, 0.8f, 1f);
    [SerializeField]
    private float roomLightIntensity = 0.2f;
    [SerializeField]
    private float roomLightRange = 8f;
    [SerializeField]
    private GameObject ceilingLightPrefab;
    [SerializeField]
    private float ceilingLightHeight = 4.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildDungeon();
        DungeonFloorCollider();
        BuildLighting();

        // Build NavMesh after dungeon is built to allow for dynamic pathfinding
        surface.BuildNavMesh();

    }


    // Build the dungeon in the Unity scene
    void BuildDungeon()
    {
        var grid = generator.GetGrid();

        AddWalls(grid, generator.dungeonWidth, generator.dungeonHeight);

        for (int x = 0; x < generator.dungeonWidth; x++)
        {
            for (int y = 0; y < generator.dungeonHeight; y++)
            {
                Vector3 wallPos = new Vector3(x, 0, y);
                Vector3 ceilingPos = new Vector3(x, wallHeight / 2, y);

                if (grid[x, y] == BSPDungeonGenerator.GridCell.Floor || grid[x, y] == BSPDungeonGenerator.GridCell.Corridor)
                {
                    Instantiate(floorPrefab, wallPos, Quaternion.identity, transform);
                    Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity, transform);
                }

                else if (grid[x, y] == BSPDungeonGenerator.GridCell.Wall)
                {
                    GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                    // randomize the y offset using gradual steps for the wall textures
                    // this variance is to prevent uniform textures
                    Renderer tex = wall.GetComponent<Renderer>();
                    float[] offsets = { 0f, 0.08f, 0.16f };
                    float yOffset = offsets[Random.Range(0, offsets.Length)];
                    tex.material.mainTextureOffset = new Vector2(0f, yOffset);

                }
            }
        }
    }

    void AddWalls(BSPDungeonGenerator.GridCell[,] grid, int dungeonWidth, int dungeonHeight)
    {
        // Loop through the grid
        for (int x = 1; x < dungeonWidth - 1; x++)
        {
            for (int y = 1; y < dungeonHeight - 1; y++)
            {
                // If this tile is a floor or corridor
                if (grid[x, y] == BSPDungeonGenerator.GridCell.Floor || grid[x, y] == BSPDungeonGenerator.GridCell.Corridor)
                {
                    // Check surrounding tiles
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            // Only place walls into empty tiles 
                            if (grid[x + dx, y + dy] == BSPDungeonGenerator.GridCell.Empty)
                            {
                                grid[x + dx, y + dy] = BSPDungeonGenerator.GridCell.Wall;
                            }
                        }
                    }
                }
            }
        }
    }

    void DungeonFloorCollider()
    {
        GameObject floorCollider = new GameObject("Floor Collider");
        floorCollider.transform.SetParent(transform); // set as child of dungeon builder object
        // creates a box colider thats the size of the entire dungeon and center it
        BoxCollider floorColliderComponent = floorCollider.AddComponent<BoxCollider>();
        floorColliderComponent.size = new Vector3(generator.dungeonWidth, 1f, generator.dungeonHeight);
        floorColliderComponent.center = new Vector3((generator.dungeonWidth - 1) / 2f, -0.5f, (generator.dungeonHeight - 1) / 2f);
    }


    // create a point light at the center of each room and place a ceilight light prefab
    void BuildLighting()
    {
        BSPNode root = generator.GetRoot();
        List<RectInt> rooms = BSPHelper.GetAllRooms(root);

        for (int i = 0; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            Vector2Int center = BSPHelper.GetRoomCenter(room);
            Vector3 pos = new Vector3(center.x, lightHeight, center.y);
            Vector3 ceilingLightPos = new Vector3(center.x, ceilingLightHeight, center.y);

            CreatePointLight("Room Light", pos, roomLightIntensity, roomLightRange);
            Instantiate(ceilingLightPrefab, ceilingLightPos, Quaternion.identity, transform);
        }
    }

    void CreatePointLight(string lightName, Vector3 position, float intensity, float range)
    {
        GameObject lightObject = new GameObject(lightName);
        lightObject.transform.SetParent(transform);
        lightObject.transform.position = position;

        Light pointLight = lightObject.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.intensity = intensity;
        pointLight.range = range;
        pointLight.color = lightColor;
        pointLight.shadows = LightShadows.None;
    }

}
