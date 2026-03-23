using UnityEngine;
using System.Collections.Generic;

public class BSPDungeonGenerator : MonoBehaviour
{
    //Size of the entire dungeon
    [SerializeField]
    public int dungeonWidth = 64;
    [SerializeField]
    public int dungeonHeight = 64;

    //Size constraints of each partition
    [SerializeField]
    public int minPartitionSize = 12;
    [SerializeField]
    public int maxPartitionSize = 20;

    //Min size of room in a partition
    [SerializeField]
    public int minRoomSize = 6;

    // Padding between room and partition edge
    [SerializeField]
    private int roomPadding = 2;

    // Width of corridors connecting rooms
    [SerializeField]
    private int corridorWidth = 3;

    [SerializeField]
    private int maxDepth = 5; // Number of nodes is equal to 2^(depth+1) - 1

    // Chance of spawning a room in a partition
    [SerializeField, Range(0f, 1f)]
    private float roomSpawnChance = 0.5f;

    GridCell[,] grid;
    BSPNode root;

    private RectInt spawnRoom;

    // Represents the type of each cell in the dungeon grid
    public enum GridCell
    {
        Empty, // Non playable area
        Floor, // Walkable area
        Corridor, // Pathway
        Wall // Boundary
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        grid = new GridCell[dungeonWidth, dungeonHeight];

        root = new BSPNode(new RectInt(0, 0, dungeonWidth, dungeonHeight));
        SplitNode(root, 0);

        CreateRooms(root);
        ConnectRooms(root);

    }

    void SplitNode(BSPNode node, int depth)
    {
        // limit the depth of the BSP tree
        if (depth >= maxDepth)
        {
            return;
        }

        // stop if the partition is smaller than the max size
        if (node.partition.width <= maxPartitionSize && node.partition.height <= maxPartitionSize)
        {
            return;
        }

        // Generate a random value between 0 and 1 , if greater than 0.5 return true 
        // A 50/50 Check if we should split horizontally or vertically
        bool splitHorizontal = Random.value > 0.5f;


        // If the partition is significantly wider than it is tall, split vertically (1.25f me checking if 25% wider)
        float ratio = (float)node.partition.width / (float)node.partition.height;
        if (node.partition.width > node.partition.height && ratio >= 1.25f)
        {
            splitHorizontal = false;
        }
        // If the partition is significantly taller than it is wide, split horizontally
        else if (node.partition.height > node.partition.width && (1f / ratio) >= 1.25f)
        {
            splitHorizontal = true;
        }

        int max;
        // Ensure the partition meets the minimum size requirements
        if (splitHorizontal == true)
        {
            max = node.partition.height - minPartitionSize;
        }
        else
        {
            max = node.partition.width - minPartitionSize;
        }

        // Do not split if the max split size is less than the min partition size
        if (max < minPartitionSize)
        {
            return;
        }

        // randomly determine where to split the partition
        int split = Random.Range(minPartitionSize, max + 1); // +1 because the max value is exclusive for Random,Range unity

        // Split the partition into two child partitions horizontally
        if (splitHorizontal == true)
        {
            // Bottom child
            node.firstChild = new BSPNode(new RectInt(node.partition.x, node.partition.y, node.partition.width, split));
            // Top child
            node.secondChild = new BSPNode(new RectInt(node.partition.x, node.partition.y + split, node.partition.width, node.partition.height - split));
        }
        // Split the partition into two child partitions vertically
        else
        {
            // Left child
            node.firstChild = new BSPNode(new RectInt(node.partition.x, node.partition.y, split, node.partition.height));
            // Right child
            node.secondChild = new BSPNode(new RectInt(node.partition.x + split, node.partition.y, node.partition.width - split, node.partition.height));
        }

        // recursively split the child partitions
        SplitNode(node.firstChild, depth + 1);
        SplitNode(node.secondChild, depth + 1);
    }

    void CreateRooms(BSPNode node)
    {
        // Check if the node doesnt have any children (leaf node)
        if (node.IsLeaf() == false)
        {
            CreateRooms(node.firstChild);
            CreateRooms(node.secondChild);
            return;
        }

        // Set room to default values to represent no room
        if (Random.value > roomSpawnChance)
        {
            node.room = default;
            return;
        }

        // Create random room dimensions based on minRoomSize and the size of the partition
        int roomWidth = Random.Range(minRoomSize, node.partition.width - 2 * roomPadding);
        int roomHeight = Random.Range(minRoomSize, node.partition.height - 2 * roomPadding);

        // Create random position for the room within the partition
        int roomX = Random.Range(node.partition.x + roomPadding, node.partition.x + node.partition.width - roomWidth - roomPadding);
        int roomY = Random.Range(node.partition.y + roomPadding, node.partition.y + node.partition.height - roomHeight - roomPadding);
        node.room = new RectInt(roomX, roomY, roomWidth, roomHeight);

        // Loop through the grid and set the tiles within the room to Floor
        for (int x = roomX; x < roomX + roomWidth; x++)
        {
            for (int y = roomY; y < roomY + roomHeight; y++)
            {
                grid[x, y] = GridCell.Floor;
            }
        }
    }

    // Connects room in the BSP tree 
    void ConnectRooms(BSPNode node)
    {

        if (node == null)
        {
            return;
        }

        if (node.IsLeaf())
        {
            return;
        }

        var left = BSPHelper.TryGetRoom(node.firstChild);
        var right = BSPHelper.TryGetRoom(node.secondChild);

        if (left.found && right.found)
        {
            Vector2Int start = BSPHelper.GetRoomCenter(left.room);
            Vector2Int end = BSPHelper.GetRoomCenter(right.room);

            CarveCorridor(start, end);
        }

        ConnectRooms(node.firstChild);
        ConnectRooms(node.secondChild);
    }

    // Sets grid cells to Corridor and carves a path between two points
    void CarveCorridor(Vector2Int current, Vector2Int destination)
    {
        // Stop creating corridor when we reach the destination
        while (current.x != destination.x)
        {
            PaintCorridorAt(current);

            // Move right or left
            if (current.x < destination.x)
            {
                current.x = current.x + 1;
            }
            else
            {
                current.x = current.x - 1;
            }
        }

        while (current.y != destination.y)
        {
            PaintCorridorAt(current);
            // Move up or down
            if (current.y < destination.y)
            {
                current.y = current.y + 1;
            }
            else
            {
                current.y = current.y - 1;
            }
        }
    }

    // Set grid cells to Corridor based on corridor width
    void PaintCorridorAt(Vector2Int centre)
    {

        int halfWidth = corridorWidth / 2;

        for (int dx = -halfWidth; dx <= halfWidth; dx++)
        {
            for (int dy = -halfWidth; dy <= halfWidth; dy++)
            {
                int x = centre.x + dx;
                int y = centre.y + dy;

                // Check if within dungeon bounds
                if (x < 0 || x >= dungeonWidth || y < 0 || y >= dungeonHeight)
                {
                    continue;
                }
                else
                {
                    grid[x, y] = GridCell.Corridor;
                }
            }
        }
    }


    public RectInt GetRandomRoom()
    {
        return BSPHelper.GetRandomRoom(root);
    }

    //getter methods
    public int GetWidth()
    {
        return dungeonWidth;
    }

    public int GetHeight()
    {
        return dungeonHeight;
    }

    public GridCell[,] GetGrid()
    {
        return grid;
    }

    public BSPNode GetRoot()
    {
        return root;
    }

    
    public void SetSpawnRoom(RectInt room)
    {
        spawnRoom = room;
    }

    public RectInt GetSpawnRoom()
    {
        return spawnRoom;
    }

}
