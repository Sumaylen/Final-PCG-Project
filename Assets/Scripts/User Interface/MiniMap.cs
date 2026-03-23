using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{

    // only one should be assigned depending on which generator is in use
    [SerializeField] 
    private BSPDungeonGenerator bsp;
    [SerializeField] 
    private CADungeonGenerator ca;

    
    [SerializeField] 
    private RawImage mapImage;            
    [SerializeField] 
    private RectTransform playerDot;      
    [SerializeField] 
    private RectTransform portalDot;      
    [SerializeField] 
    private Transform player;             
    [SerializeField] 
    private Transform portal;            

    [SerializeField] 
    private Color floorColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] 
    private Color wallColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    [SerializeField] 
    private Color corridorColor = new Color(0.55f, 0.55f, 0.85f, 1f);
    [SerializeField] 
    private Color emptyColor = new Color(0f, 0f, 0f, 0.7f);

    // create new world origin for the minimap
    [SerializeField] private Vector2 worldOrigin = Vector2.zero; 

    private Texture2D mapTexture;
    private int mapWidth;
    private int mapHeight;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        
        // check if we using bsp or ca generator and then build minimap accordingly
        if (bsp != null)
        {
            portalDot.gameObject.SetActive(true);
            portal = GameObject.FindWithTag("Portal").transform;
            BSPDungeonGenerator.GridCell[,] grid = bsp.GetGrid();
            mapWidth = bsp.GetWidth();
            mapHeight = bsp.GetHeight();

            BuildTexture(mapWidth, mapHeight);
            DrawBSP(grid);
            mapTexture.Apply(false);
            UpdateDot(portal, portalDot);
        }
        else
        {
            portalDot.gameObject.SetActive(false);
            int[,] map = ca.GetMap();
            mapWidth = ca.GetWidth();
            mapHeight = ca.GetHeight();

            BuildTexture(mapWidth, mapHeight);
            DrawCA(map);
            mapTexture.Apply(false);
        }

    }

    void Update()
    {
        UpdateDot(player, playerDot);
    }

    void BuildTexture(int w, int h)
    {
        mapTexture = new Texture2D(w, h, TextureFormat.RGBA32, false);
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;

        mapImage.texture = mapTexture;
    }

    void DrawBSP(BSPDungeonGenerator.GridCell[,] grid)
    {
        // loop through the grid and set pixel colors based on grid type
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                BSPDungeonGenerator.GridCell cell = grid[x, y];
                Color col = emptyColor;
                switch (cell)
                {   
                    // only draw corridors if not inside a room
                    case BSPDungeonGenerator.GridCell.Corridor:
                        if (IsRoom(bsp.GetRoot(), x, y))
                        {
                            col = floorColor;
                        }
                        else
                        {
                            col = corridorColor;
                        }
                        break;
                    case BSPDungeonGenerator.GridCell.Floor:
                        col = floorColor;
                        break;
                    case BSPDungeonGenerator.GridCell.Wall:
                        col = wallColor;
                        break;
                    default:
                        col = emptyColor;
                        break;
                }

                mapTexture.SetPixel(x, y, col);
            }
        }
    }

    bool IsRoom(BSPNode node, int x, int y)
    {
        if (node == null)
        {
            return false;
        }

        // rooms can only be found in leaf node, so check if tile is inside a room and return true if it is
        if (node.IsLeaf() && node.room.Contains(new Vector2Int(x, y)))
        {
            return true;
        }

        // recursively check children if either contains a room tile
        return IsRoom(node.firstChild, x, y) || IsRoom(node.secondChild, x, y);
    }

    void DrawCA(int[,] map)
    {   
        // loop through the map and set pixel colors based on cell type
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 0 = floor, 1 = wall
                if (map[x, y] == 0)
                {
                    mapTexture.SetPixel(x, y, floorColor);
                }
                else
                {
                    mapTexture.SetPixel(x, y, wallColor);
                }
            }
        }
    }

    void UpdateDot(Transform worldTarget, RectTransform dot)
    {
   
        // get the size of the minimap
        RectTransform mapRect = mapImage.rectTransform;
        Vector2 size = mapRect.rect.size;

        // Convert targets world position into map local position
        float gridX = worldTarget.position.x - worldOrigin.x;
        float gridY = worldTarget.position.z - worldOrigin.y;

        // Normalize into percentage position 
        float normalizedX = gridX / mapWidth;
        float normalizedY = gridY / mapHeight;

        // converts from normalized position to ui position with an offset (offset is needed because anchored position is relative to the center)
        float uiX = (normalizedX - 0.5f) * size.x;
        float uiY = (normalizedY - 0.5f) * size.y;

        // anchored position is relative to parent RawImage
        dot.anchoredPosition = new Vector2(uiX, uiY);
    }
}
