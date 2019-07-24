using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    public NavMeshSurface surface;
    public GameObject parentObj;
    GameObject dungeon;

    private void Start()
    {
        StartCoroutine(ResetLevel());
        
        
    }
    //positive Z axis = north
    public void GenerateLevel()
    {
        
        RoomTemplates templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        GameObject room = templates.startRoom;
        GameObject o = Instantiate(room, Vector3.zero, room.transform.rotation);
        o.transform.SetParent(GameObject.FindGameObjectWithTag("DungeonParent").transform);

        

        Invoke("UpdateNavigation", 5f);

    }
    IEnumerator ResetLevel()
    {
        dungeon = Instantiate(parentObj, Vector3.zero, Quaternion.identity);
        GenerateLevel();
        yield return new WaitForSeconds(7f);
        Destroy(dungeon);
        
        StartCoroutine("ResetLevel");
    }
    
   
}
