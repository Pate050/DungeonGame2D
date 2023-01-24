using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all the data about our dungeon.
/// Useful when creating a save / load system
/// </summary>
public class DungeonData : MonoBehaviour
{
    public Vector2Int PlayerStart;
    public List<Room> Rooms { get; set; } = new List<Room>();
    public HashSet<Vector2Int> Path { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> FloorTiles { get; set; } = new HashSet<Vector2Int>();

    public GameObject PlayerReference { get; set; }
    public void Reset()
    {
        foreach (Room room in Rooms)
        {
            foreach (var item in room.PropObjectReferences)
            {
                DestroyImmediate(item);
            }
            foreach (var item in room.EnemiesInTheRoom)
            {
                DestroyImmediate(item);
            }
        }
        Rooms = new();
        Path = new();
        DestroyImmediate(PlayerReference);
    }

    public IEnumerator TutorialCoroutine(Action code)
    {
        yield return new WaitForSeconds(1);
        code();
    }
}


/// <summary>
/// Holds all the data about the room
/// </summary>
public class Room
{
    public Vector2Int RoomCenterPos { get; set; }
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> PropPositions { get; set; } = new HashSet<Vector2Int>();
    public List<GameObject> PropObjectReferences { get; set; } = new List<GameObject>();

    public List<Vector2Int> PositionsAccessibleFromPath { get; set; } = new List<Vector2Int>();

    public List<GameObject> EnemiesInTheRoom { get; set; } = new List<GameObject>();

    public string RoomType { get; set; }

    public Room(Vector2Int roomCenterPos, HashSet<Vector2Int> floorTiles)
    {
        this.RoomCenterPos = roomCenterPos;
        this.FloorTiles = floorTiles;
        this.RoomType = "";
    }
}