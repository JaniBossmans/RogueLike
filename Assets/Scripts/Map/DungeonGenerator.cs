using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private int width, height;
    private int maxRoomSize, minRoomSize;
    private int maxRooms;
    private int maxEnemies;
    private int maxItems;
    private int currentFloor;

    List<Room> rooms = new List<Room>();
    private List<string> enemyNames = new List<string> { "Dwarf", "Spider", "Pigman", "Eye", "Wolf", "Cock_Dragon", "Wasp", "Tiger", "Woman", "Ghost" };

    public void SetSize(int width, int height)
    {
        this.width = width;
    }

    public void SetRoomSize(int min, int max)
    {
        minRoomSize = min;
        maxRoomSize = max;
    }

    public void SetMaxRooms(int max)
    {
        maxRooms = max;
    }

    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }

    public void SetMaxItems(int max)
    {
        maxItems = max;
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
    }

    public void Generate()
    {
        rooms.Clear();
        ClearTilemaps();

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);

            int roomX = Random.Range(0, width - roomWidth - 1);
            int roomY = Random.Range(0, height - roomHeight - 1);

            var room = new Room(roomX, roomY, roomWidth, roomHeight);

            if (room.Overlaps(rooms))
            {
                continue;
            }

            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (!TrySetWallTile(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y, 0));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                TunnelBetween(rooms[rooms.Count - 1], room);
            }
            PlaceEnemies(room, maxEnemies);
            PlaceItems(room, maxItems);
            rooms.Add(room);
        }

        var lastRoomCenter = rooms[rooms.Count - 1].Center();
        GameManager.Get.CreateGameObject("Ladder_Down", new Vector3(lastRoomCenter.x, lastRoomCenter.y, 0));

        var player = GameManager.Get.FindPlayer();
        if (player != null)
        {
            player.transform.position = new Vector3(rooms[0].Center().x, rooms[0].Center().y, 0);
        }
        else
        {
            GameManager.Get.CreateGameObject("Player", new Vector3(rooms[0].Center().x, rooms[0].Center().y, 0));
        }

        if (currentFloor > 0)
        {
            var firstRoomCenter = rooms[0].Center();
            GameManager.Get.CreateGameObject("Ladder_Up", new Vector3(firstRoomCenter.x, firstRoomCenter.y, 0));
        }
    }

    private bool TrySetWallTile(Vector3Int pos)
    {
        if (MapManager.Get.FloorMap.GetTile(pos))
        {
            return false;
        }
        else
        {
            MapManager.Get.ObstacleMap.SetTile(pos, MapManager.Get.WallTile);
            return true;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.Get.ObstacleMap.GetTile(pos))
        {
            MapManager.Get.ObstacleMap.SetTile(pos, null);
        }
        MapManager.Get.FloorMap.SetTile(pos, MapManager.Get.FloorTile);
    }

    private void TunnelBetween(Room oldRoom, Room newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0));

            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (!TrySetWallTile(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private void PlaceEnemies(Room room, int maxEnemies)
    {
        int numEnemies = Random.Range(0, maxEnemies + 1);
        float difficultyMultiplier = Mathf.Clamp01((float)currentFloor / 10f);

        for (int i = 0; i < numEnemies; i++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            int enemyIndex = Mathf.FloorToInt(Random.Range(0, enemyNames.Count) * difficultyMultiplier);
            string enemyName = enemyNames[enemyIndex];

            GameManager.Get.CreateGameObject(enemyName, new Vector2(x, y));
        }
    }

    private void PlaceItems(Room room, int maxItems)
    {
        int num = Random.Range(0, maxItems + 1);

        for (int counter = 0; counter < num; counter++)
        {
            int x = Random.Range(room.X + 1, room.X + room.Width - 1);
            int y = Random.Range(room.Y + 1, room.Y + room.Height - 1);

            if (Random.value < 0.5f)
            {
                GameManager.Get.CreateGameObject("HealthPotion", new Vector2(x, y));
            }
            else
            {
                GameManager.Get.CreateGameObject("Fireball", new Vector2(x, y));
            }
        }
    }

    private void ClearTilemaps()
    {
        MapManager.Get.FloorMap.ClearAllTiles();
        MapManager.Get.ObstacleMap.ClearAllTiles();
        MapManager.Get.FogMap.ClearAllTiles();
    }
}
