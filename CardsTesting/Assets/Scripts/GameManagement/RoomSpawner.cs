using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    //public int doorDirection = 1;
    public enum directions
    {
        north, east, south, west
    }
    public directions direction = directions.north;
    RoomTemplates templates;
    int r;
    int roomsSpawned = 0;
    private bool spawned;
    GameObject dungeonParent;
    float waitTime = 4f;

    private void Start()
    {

        dungeonParent = GameObject.FindGameObjectWithTag("DungeonParent");
        //Destroy(gameObject, waitTime);
        dungeonParent = GameObject.FindGameObjectWithTag("DungeonParent");
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        //Random.InitState(2);

        if(spawn) Invoke("SpawnRoom", .1f);
    }

    void SpawnRoom()
    {
        dungeonParent = GameObject.FindGameObjectWithTag("DungeonParent");
        if (!spawned && spawn)
        {
            
            GameObject o;
            GameObject room = null;
            Vector3 pos = transform.position;


            switch (direction)
            {
                case directions.south: // bottom door required

                    r = Random.Range(0, templates.bottomRooms.Length);
                    room = templates.bottomRooms[r];
                    o = Instantiate(room, transform.position, room.transform.rotation);
                    o.transform.SetParent(dungeonParent.transform);
                    break;
                case directions.north: // top door required

                    r = Random.Range(0, templates.topRooms.Length);
                    room = templates.topRooms[r];
                    o = Instantiate(room, transform.position, room.transform.rotation);
                    o.transform.SetParent(dungeonParent.transform);
                    break;
                case directions.west: // left door required

                    r = Random.Range(0, templates.leftRooms.Length);
                    room = templates.leftRooms[r];
                    o = Instantiate(room, transform.position, room.transform.rotation);
                    o.transform.SetParent(dungeonParent.transform);
                    break;
                case directions.east: // right door required

                    r = Random.Range(0, templates.rightRooms.Length);
                    room = templates.rightRooms[r];
                    o = Instantiate(room, transform.position, room.transform.rotation);
                    o.transform.SetParent(dungeonParent.transform);
                    break;
            }
            spawned = true;



            roomsSpawned++;
        }
        
    }
    bool spawn;
    int nearbyRooms;
    private void OnTriggerEnter(Collider other)
    {
        if (spawn)
        {
            if (other.CompareTag("Room"))
            {
                Debug.Log("Found Room.");
            }
            else if (other.CompareTag("SpawnPoint"))// || other.CompareTag("Start"))
            {

                if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
                {
                    Invoke("SpawnWall", 2f);
                    //Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                    //Debug.Log("Found corner. " + transform.position);
                    //Destroy(gameObject);
                }
                spawned = true;
                //other.GetComponent<RoomSpawner>().spawned = true;



            }
        }

    }

    void SpawnWall()
    {
        Debug.Log("Checking...");
        if (spawned == false)
        {
            Debug.Log("Filling...");
            Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
        }
        
    }
}
