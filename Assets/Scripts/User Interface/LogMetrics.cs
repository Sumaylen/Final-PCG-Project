using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LogMetrics : MonoBehaviour
{
    [SerializeField]
    private BSPDungeonGenerator bspGenerator;
    [SerializeField]
    private EnemySpawnerBSP bspSpawner;
    [SerializeField]
    private CADungeonGenerator caGenerator;

    public string GetMetricsText()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 1)
        {
            return GetBSPMetrics();
        }
        else
        {
            return GetCAMetrics();
        }
    }

    string GetBSPMetrics()
    {
        List<RectInt> rooms = BSPHelper.GetAllRooms(bspGenerator.GetRoot());
        int roomCount = rooms.Count;

        BSPDungeonGenerator.GridCell[,] grid = bspGenerator.GetGrid();
        int corridorTiles = 0;

        for (int x = 0; x < bspGenerator.dungeonWidth; x++)
        {
            for (int y = 0; y < bspGenerator.dungeonHeight; y++)
            {
                if (grid[x, y] == BSPDungeonGenerator.GridCell.Corridor)
                {
                    corridorTiles++;
                }
            }
        }

        int enemiesAlive = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;

        return "Generator Used: BSP"
            + "\nRooms: " + roomCount
            + "\nCorridor Tiles: " + corridorTiles
            + "\nEnemies Alive: " + enemiesAlive
            + "\nElite Spawn Chance: " + (bspSpawner.GetEliteSpawnChance() * 100f).ToString("0") + "%"
            + "\nTotal Kills: " + KillCounter.GetKills();
    }

    string GetCAMetrics()
    {
        int[,] map = caGenerator.GetMap();
        int width = caGenerator.GetWidth();
        int height = caGenerator.GetHeight();

        int openTiles = 0;
        int totalTiles = width * height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    openTiles++;
                }
            }
        }

        float openPercent = (openTiles / (float)totalTiles) * 100f;
        int enemiesAlive = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;

        return "Generator Used: CA"
            + "\nOpen Map %: " + openPercent.ToString("0.0") + "%"
            + "\nEnemies Alive: " + enemiesAlive
            + "\nTotal Kills: " + KillCounter.GetKills();
    }
}
