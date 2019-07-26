using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public Room.directions currentDirection = Room.directions.north;
    public int mapWidth = 10, mapHeight = 10;
    public float gridSize = 14.5f;
    public Vector3 startingPosition = Vector3.zero;
    public Vector3 currentRoomPosition;
    public GameObject cubePrefab;
    public GameObject lastRoomOBJ;
    public Room lastRm;
    public Room[,] map;

    private void Start()
    {
        map = new Room[mapWidth, mapHeight];
        currentRoomPosition = startingPosition;


        StartRoom();
        ConnectedRooms();
    }
    void StartRoom()
    {
        // fill the center *start* room
        int center = mapWidth / 2;
        map[center, center] = new Room(true, false);
        map[center, center].dIndex = Random.Range(0, 4);
        Room.directions z = Room.directions.north;
        switch (map[center, center].dIndex)
        {
            case 0: z = Room.directions.north; break;
            case 1: z = Room.directions.east; break;
            case 2: z = Room.directions.south; break;
            case 3: z = Room.directions.west; break;
        }
        lastRoomOBJ = SpawnRoom(z, map[center, center].position);
        map[center, center].roomPrefab = lastRoomOBJ;
        lastRm = map[center, center];
    }
    void ConnectedRooms()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            currentRoomPosition.x += gridSize;

            for (int y = 0; y < mapHeight; y++)
            {
                currentRoomPosition.z += gridSize;

                bool[] walls = new bool[4];
                map[x, y] = new Room(Room.directions.north, walls);

                currentRoomPosition.y = 0;
                map[x, y].position = currentRoomPosition;
                map[x, y].nodePreview = Instantiate(cubePrefab, currentRoomPosition, Quaternion.identity);
                List<Transform> spawns = new List<Transform>();
                foreach(Transform child in lastRoomOBJ.transform)
                {
                    spawns.Add(child);
                }
                int r = Random.Range(0, spawns.Count);
                lastRoomOBJ = SpawnRoom(lastRm.direction, spawns[r].position);
                map[x, y].roomPrefab = lastRoomOBJ;
                if (y == mapHeight - 1)
                    currentRoomPosition.z = 0;
            }
        }
    }
    GameObject SpawnRoom(Room.directions direction, Vector3 pos)
    {
        RoomTemplates templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        GameObject dungeonParent = GameObject.FindGameObjectWithTag("DungeonParent");
        GameObject o = null;
        GameObject room = null;
        
        int r;

        switch (direction)
        {
            case Room.directions.south: // bottom door required

                r = Random.Range(0, templates.bottomRooms.Length);
                room = templates.bottomRooms[r];

                break;
            case Room.directions.north: // top door required

                r = Random.Range(0, templates.topRooms.Length);
                room = templates.topRooms[r];

                break;
            case Room.directions.west: // left door required

                r = Random.Range(0, templates.leftRooms.Length);
                room = templates.leftRooms[r];

                break;
            case Room.directions.east: // right door required

                r = Random.Range(0, templates.rightRooms.Length);
                room = templates.rightRooms[r];

                break;
        }

        o = Instantiate(room, pos, room.transform.rotation);
        o.transform.SetParent(dungeonParent.transform);

        return o;

    }
}


public class Room 
{
    bool startRoom, endRoom;
    public float fcost = 1f;
    public GameObject nodePreview;
    public GameObject roomPrefab;
    public int dIndex = 0;
    public enum directions
    {
        north, east, south, west
    }
    public directions direction = directions.north;
    public bool[] walls = new bool[4]; // clockwise starting with North as 0
    

    public Vector3 position = Vector3.zero;

    public Room(directions direction, bool[] walls)
    {
        this.direction = direction;
        this.walls = walls;
    }
    public Room(bool start, bool end)
    {
        startRoom = start;
        endRoom = end;
    }

}


