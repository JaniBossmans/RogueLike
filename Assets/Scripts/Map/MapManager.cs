using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static MapManager Get { get => instance; }

    [Header("TileMaps")]
    public Tilemap FloorMap;
    public Tilemap ObstacleMap;
    public Tilemap FogMap;

    [Header("Tiles")]
    public TileBase FloorTile;
    public TileBase WallTile;
    public TileBase FogTile;

    [Header("Features")]
    public Dictionary<Vector2Int, Node> Nodes = new Dictionary<Vector2Int, Node>();
    public List<Vector3Int> VisibleTiles;
    public Dictionary<Vector3Int, TileData> Tiles;

    [Header("Map Settings")]
    public int width = 80;
    public int height = 45;
    public int roomMaxSize = 10;
    public int roomMinSize = 6;
    public int maxRooms = 30;
    public int maxItems = 2; // Standaardwaarde voor maxItems
    public int maxEnemies = 10; // Standaardwaarde voor maxEnemies
    public int floor = 0;

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        ClearObjects(); // New method to clear objects
        Tiles = new Dictionary<Vector3Int, TileData>();
        VisibleTiles = new List<Vector3Int>();

        GameObject dungeonGeneratorObject = new GameObject("DungeonGenerator");
        DungeonGenerator generator = dungeonGeneratorObject.AddComponent<DungeonGenerator>();

        generator.SetSize(width, height);
        generator.SetRoomSize(roomMinSize, roomMaxSize);
        generator.SetMaxRooms(maxRooms);
        generator.SetMaxItems(maxItems); // Gebruik maxItems van MapManager
        generator.SetMaxEnemies(maxEnemies); // Gebruik maxEnemies van MapManager
        generator.SetCurrentFloor(floor); // Set current floor
        generator.Generate();

        // Update floor info
        UIManager.Get.UpdateFloor(floor);
    }

    private void ClearObjects()
    {
        GameManager.Get.ClearAllObjects(); // Clear enemies and items
        FogMap.ClearAllTiles(); // Clear fog tiles
    }

    public void MoveUp()
    {
        floor--;
        if (floor < 0)
        {
            floor = 0; // Prevent going below the first floor
        }
        GenerateDungeon();
    }

    public void MoveDown()
    {
        floor++;
        GenerateDungeon();
    }

    public bool IsWalkable(Vector3 position)
    {
        Vector3Int cellPosition = FloorMap.WorldToCell(position);
        return FloorMap.HasTile(cellPosition) && !ObstacleMap.HasTile(cellPosition);
    }

    public void UpdateFogMap(List<Vector3Int> fieldOfView)
    {
        FogMap.ClearAllTiles();
        foreach (var tilePos in fieldOfView)
        {
            FogMap.SetTile(tilePos, null); // Clear fog of war from the visible tiles
        }
    }
}
