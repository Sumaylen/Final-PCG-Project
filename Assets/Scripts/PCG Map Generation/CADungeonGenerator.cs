using UnityEngine;
using Unity.AI.Navigation;

public class CADungeonGenerator : MonoBehaviour
{
    //Size of the entire dungeon
    [SerializeField]
    private int width = 64;
    [SerializeField]
    private int height = 64;
    // initial randomness, more means more walls
    [SerializeField, Range(0, 100)]
    private int fillProb = 45;
    // how many times to run the smoothing function
    [SerializeField]
    private int smoothingSteps = 5;
    // how many surrounding walls are needed for a cell to become a wall
    [SerializeField]
    private int wallRequirement = 4;

    [SerializeField]
    private GameObject floorPrefab;
    [SerializeField]
    private GameObject wallPrefab;
     
     [SerializeField]
    private NavMeshSurface surface;


    // 2D array representing the dungeon layout where 0 = floor and 1 = wall
    private int[,] map;
    [SerializeField]
    private Transform root;

    private void Start()
    {
        GenerateDungeon();
    }


    public void GenerateDungeon()
    {
        BuildInitialMap();

        for (int i = 0; i < smoothingSteps; i++)
        {
            SmoothMap();
        }

        Build3DMap();
        surface.BuildNavMesh();
    }

    // create initial random map
    private void BuildInitialMap()
    {
        map = new int[width, height];

        // loop through each cell in the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // check if its a border cell and make it a wall if so
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                // else randomly decicde if cell is a wall or floor
                else
                {
                    if (Random.Range(0, 100) < fillProb)
                    {
                        map[x, y] = 1; //wall
                    }
                    else
                    {
                        map[x, y] = 0; //floor
                    }
                }
            }
        }
    }


    // for eeach cell on the map, count how many surrounding walls it has and decide if it should be a wall or floor
    private void SmoothMap()
    {
        // create a new map
        int[,] nextMap = new int[width, height];

        // loop through each cell in the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingWalls = GetSurroundingWallCount(x, y);

                // if there are more surrounding walls than the requirement, make this cell a wall
                if (surroundingWalls > wallRequirement)
                {
                    nextMap[x, y] = 1;
                }
                // else if there are fewer surrounding walls than the requirement, make this cell a floor
                else if (surroundingWalls < wallRequirement)
                {
                    nextMap[x, y] = 0;
                }
                // else keep the cell the same
                else
                {
                    nextMap[x, y] = map[x, y];
                }
            }
        }
        // replace the old map with the new smoothed map
        map = nextMap;
    }

    // count how many surrounding walls a cell has
    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        // loop through the 3x3 grid around the cell at gridX, gridY
        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                // skip the middle cell itself
                if (x == gridX && y == gridY)
                {
                    continue;
                }
                // if cell is out of bounds count it as a wall
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    wallCount++;
                }
                // else add the value of the cell to the wall count (0 for floor, 1 for wall)
                else
                {
                    wallCount += map[x, y];
                }
            }
        }

        return wallCount;
    }

    // loop through the map and instantiate floor and wall prefabs based on the values in the map
    private void Build3DMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0f, y);

                if (map[x, y] == 0)
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, root);
                }
                else
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, root);
                }
            }
        }
    }

    public Vector3 GetRandomFloorPosition()
    {
        // Try random positions first
        for (int tries = 0; tries < 200; tries++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);

            if (map[x, y] == 0) 
            {
                return new Vector3(x, 0f, y);
            }
        }

        // slower saftey check if all random positions are walls
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (map[x, y] == 0)
                {
                    return new Vector3(x, 0f, y);
                }
            }
        }

        return Vector3.zero; //default, should never happen
    }

// getter methods
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public int[,] GetMap()
    {
        return map;
    }

}
