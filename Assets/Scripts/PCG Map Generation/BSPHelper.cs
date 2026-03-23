using UnityEngine;
using System.Collections.Generic;

public class BSPHelper : MonoBehaviour
{
    // Helper function to get centre of the room,
    public static Vector2Int GetRoomCenter(RectInt room)
    {
        return new Vector2Int(
            room.x + room.width / 2,
            room.y + room.height / 2
        );
    }

    
    // Helper function that returns a random room from the dungeon
    public static RectInt GetRandomRoom(BSPNode root)
    {
        List<RectInt> rooms = GetAllRooms(root);
        return rooms[Random.Range(0, rooms.Count)];
    }

    public static List<RectInt> GetAllRooms(BSPNode root)
    {
        List<RectInt> rooms = new List<RectInt>();
        CollectRooms(root, rooms);
        return rooms;
    }

    // Recursive helper function to collect all rooms from BSP tree
    public static void CollectRooms(BSPNode node, List<RectInt> rooms)
    {
        if (node == null)
        {
            return;
        }

        if (node.IsLeaf())
        {
            if (node.room.width > 0 && node.room.height > 0)
            {
                rooms.Add(node.room);
                return;
            }
        }

        CollectRooms(node.firstChild, rooms);
        CollectRooms(node.secondChild, rooms);
    }

    // Helper function that tries to get a room from the BSP tree    
    public static (bool found, RectInt room) TryGetRoom(BSPNode node)
    {

        if (node == null)
        {
            return (false, default);
        }

        if (node.IsLeaf())
        {
            if (node.room.width > 0 && node.room.height > 0)
            {
                return (true, node.room);
            }
            else
            {
                return (false, default);
            }
        }

        var left = TryGetRoom(node.firstChild);
        if (left.found)
        {
            return left;
        }

        return TryGetRoom(node.secondChild);
    }



}
