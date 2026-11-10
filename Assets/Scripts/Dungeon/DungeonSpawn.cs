using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonSpawn : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private int mapWidth = 60;
    [SerializeField] private int mapHeight = 40;
    [SerializeField] private int minLeafSize = 10;
    [SerializeField] private int roomPadding = 2;   

    [Header("Tilemap")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    private BSPNode root;
    private readonly List<BSPNode> leaves = new List<BSPNode>();
    private readonly HashSet<Vector2Int> floorCells = new HashSet<Vector2Int>();

    private void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        leaves.Clear();
        floorCells.Clear();

        root = new BSPNode(new RectInt(0, 0, mapWidth, mapHeight));
        SplitRecursive(root);
        CreateRooms(root);
        ConnectRooms(root);
        DrawFloor();
        DrawWalls();
    }

    private void SplitRecursive(BSPNode node)
    {
        if (node.Split(minLeafSize))
        {
            SplitRecursive(node.left);
            SplitRecursive(node.right);
        }
        else
        {
            leaves.Add(node);
        }
    }

    private void CreateRooms(BSPNode node)
    {
        if (node.IsLeaf)
        {
            int w = Random.Range(4, node.area.width - roomPadding * 2 + 1);
            int h = Random.Range(4, node.area.height - roomPadding * 2 + 1);
            int x = Random.Range(node.area.x + roomPadding, node.area.xMax - roomPadding - w + 1);
            int y = Random.Range(node.area.y + roomPadding, node.area.yMax - roomPadding - h + 1);

            node.room = new RectInt(x, y, w, h);
            node.hasRoom = true;

            for (int i = x; i < x + w; i++)
                for (int j = y; j < y + h; j++)
                    floorCells.Add(new Vector2Int(i, j));
            return;
        }
        CreateRooms(node.left);
        CreateRooms(node.right);
    }

    private void ConnectRooms(BSPNode node)
    {
        if (node.IsLeaf) return;

        ConnectRooms(node.left);
        ConnectRooms(node.right);

        Vector2Int a = GetRandomRoomCenter(node.left);
        Vector2Int b = GetRandomRoomCenter(node.right);
        CreateCorridor(a, b);
    }

    private Vector2Int GetRandomRoomCenter(BSPNode node)
    {
        if (node.IsLeaf)
            return Vector2Int.RoundToInt(node.room.center);

        return Random.value > 0.5f
            ? GetRandomRoomCenter(node.left)
            : GetRandomRoomCenter(node.right);
    }

    private void CreateCorridor(Vector2Int from, Vector2Int to)
    {
        Vector2Int pos = from;

        while (pos.x != to.x)
        {
            pos.x += pos.x < to.x ? 1 : -1;
            floorCells.Add(pos);
        }
        while (pos.y != to.y)
        {
            pos.y += pos.y < to.y ? 1 : -1;
            floorCells.Add(pos);
        }
    }

    private void DrawFloor()
    {
        foreach (Vector2Int cell in floorCells)
            floorTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), floorTile);
    }

    private void DrawWalls()
    {
        foreach (Vector2Int cell in floorCells)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector2Int n = new Vector2Int(cell.x + dx, cell.y + dy);
                    if (!floorCells.Contains(n))
                        wallTilemap.SetTile(new Vector3Int(n.x, n.y, 0), wallTile);
                }
            }
        }
    }

    // Vị trí giữa phòng đầu tiên, dùng để đặt Player
    public Vector3 GetStartPosition()
    {
        Vector2Int c = Vector2Int.RoundToInt(leaves[0].room.center);
        return floorTilemap.GetCellCenterWorld(new Vector3Int(c.x, c.y, 0));
    }
}