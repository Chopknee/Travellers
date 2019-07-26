using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSwapper : MonoBehaviour
{
    public GameObject[] allRooms;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject t in allRooms)
        {
            t.gameObject.tag = "Room";
        }
        //gameObject.tag = "Room";
    }
    
}
