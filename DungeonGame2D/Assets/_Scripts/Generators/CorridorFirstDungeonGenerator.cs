using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


// Generoi Dungeonin niin ett‰ aluksi luodaan m‰‰r‰tty m‰‰r‰ halutun mittaisia k‰yt‰vi‰ 
// ja sen j‰lkeen k‰yt‰vien risteyksiin luodaan satunnaisen mallisia huoneita
public class CorridorFirstDungeonGenerator : SimpleRandomDungeonGenerator
{
    // Parametrit
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f,1)]
    private float roomPercent;
    [SerializeField]
    private List<string> roomTypeLabels;

    // Data
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary
        = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    private HashSet<Vector2Int> floorPositions, corridorPositions;
    public UnityEvent OnStartedGeneration;
    public UnityEvent OnFinishedRoomGeneration;

    public static List<Vector2Int> fourDirections = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private DungeonData dungeonData;

    private void Start()
    {
        GenerateDungeon();
    }

    protected override void RunProceduralGeneration()
    {
        OnStartedGeneration?.Invoke();

        // Luodaan objekti johon tallennetaan kaikki tarvittavat dungeonin tiedot
        dungeonData = FindObjectOfType<DungeonData>();
        if (dungeonData == null)
            dungeonData = gameObject.AddComponent<DungeonData>();

        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        // Alustetaan data
        dungeonData.Reset();

        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        // luodaan k‰yt‰v‰t
        CreateCorridors(floorPositions, potentialRoomPositions);
        dungeonData.Path.UnionWith(corridorPositions);

        // luodaan huoneet
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        // Tallennetaan lattian siainnit
        floorPositions.UnionWith(roomPositions);
        dungeonData.FloorTiles = floorPositions;

        // Piirret‰‰n lattia ja sein‰t
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        OnFinishedRoomGeneration?.Invoke();

    }
    // Luo huoneen jos huonetta ei viel‰ ole
    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach(var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false)
            {
                var deadroom = RunRandomWalk(randomWalkParameters, position);
                Room room = new Room(position, deadroom);
                dungeonData.Rooms.Add(room);

                SaveRoomData(position, deadroom);
                roomFloors.UnionWith(deadroom);
            }
        }
    }
    // Etsii k‰yt‰vien p‰‰dyt
    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach(var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach(var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }
    // Palauttaa listan huoneiden lattioiden sijainneista
    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate =
            potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomfloor = RunRandomWalk(randomWalkParameters, roomPosition);

            Room room = new Room(roomPosition, roomfloor);
            dungeonData.Rooms.Add(room);
            SaveRoomData(roomPosition, roomfloor);
            roomPositions.UnionWith(roomfloor);
        }
        return roomPositions;
    }

    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomfloor)
    {
        roomsDictionary[roomPosition] = roomfloor;
    }
    // Luodaan k‰yt‰v‰t ja otetaan ylˆs potentiaaliset huoneiden paikat
    private void CreateCorridors(HashSet<Vector2Int> floorPositions,
                                 HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0;i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition,
                                                        corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        corridorPositions = new HashSet<Vector2Int>(floorPositions);
    }

    // Palauttaa maalien siainnit j‰rjestyksess‰ l‰himm‰st‰ kauimpaan
    public List<Vector2Int> FloodFill(HashSet<Vector2Int> goalPositions, Vector2Int start)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        List<Vector2Int> visited = new List<Vector2Int>();
        List<Vector2Int> goalsVisited = new List<Vector2Int>();

        frontier.Enqueue(start);
        while(frontier.Count > 0)
        {
            Vector2Int curTile = frontier.Dequeue();
            foreach(Vector2Int neighbour in Get4Neighbours(curTile))
            {
                if (visited.Contains(neighbour) == false && 
                    frontier.Contains(neighbour) == false &&
                    dungeonData.FloorTiles.Contains(neighbour))
                {
                    frontier.Enqueue(neighbour);
                }
            }
            visited.Add(curTile);
            if (goalPositions.Contains(curTile))
            {
                goalsVisited.Add(curTile);
            }

        }
        return goalsVisited;
    }

    // Palauttaa nelj‰ viereist‰ koordinaattia 
    public List<Vector2Int> Get4Neighbours(Vector2Int coordinate)
    {
        List<Vector2Int> neighbours4directions = new List<Vector2Int>
        {
            new Vector2Int(0,1) + coordinate, new Vector2Int(1,0) + coordinate,
            new Vector2Int(0,-1) + coordinate, new Vector2Int(-1,0) + coordinate
        };
        return neighbours4directions;
    }

    // Otsikoidaan huoneet jotta tiedet‰‰n mist‰ aloitetaan ja miss‰ on maali
    public void SetTitlesToRooms(List<string> titles)
    {
        // Arvotaan aloitus huone ja merkit‰‰n nimell‰ "playerStart"
        int startRoomNum = UnityEngine.Random.Range(0, dungeonData.Rooms.Count);
        dungeonData.Rooms[startRoomNum].RoomType = "playerStart";
        dungeonData.PlayerStart = dungeonData.Rooms[startRoomNum].RoomCenterPos;

        // lista huoneiden indekseist‰
        Dictionary<Vector2Int, int> roomIds = new Dictionary<Vector2Int, int>();
        HashSet<Vector2Int> centers = new HashSet<Vector2Int>();
        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            if (i != startRoomNum)
            {
                roomIds[dungeonData.Rooms[i].RoomCenterPos] = i;
                centers.Add(dungeonData.Rooms[i].RoomCenterPos);
            }
        }

        // Selvitet‰‰n l‰himm‰t huoneet
        List<Vector2Int> roomsSortedByDistance = FloodFill(centers,
            dungeonData.Rooms[startRoomNum].RoomCenterPos);

        // Jono halutuista huoneista kauimmaÌnen ensimm‰isen‰
        Queue<string> jono = new Queue<string>();
        foreach(string title in titles)
        {
            jono.Enqueue(title);
        }

        // M‰‰r‰tyt otsikot kauimmaisille huoneille
        foreach (Vector2Int position in roomsSortedByDistance.AsEnumerable().Reverse())
        {
            int id = roomIds[position];
            if (dungeonData.Rooms[id].RoomType == "" && jono.Count > 0)
            {
                dungeonData.Rooms[id].RoomType = jono.Dequeue();
            }
        }

        // Loput huoneet merkit‰‰n nimell‰ "enemyRoom"
        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            Room room = dungeonData.Rooms[i];
            if (room.RoomType == "")
                dungeonData.Rooms[i].RoomType = "enemyRoom";
        }
    }

}
