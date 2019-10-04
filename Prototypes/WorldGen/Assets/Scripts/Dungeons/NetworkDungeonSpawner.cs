using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDungeonSpawner: MonoBehaviour {

    public GameObject prefabToSpawn;

    void Start () {
        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            Debug.Log("SPAWNING A THING!!!!!");
            NetInstanceManager.CurrentManager.Instantiate(prefabToSpawn, false, transform.position, transform.rotation);
        }
        Destroy(gameObject);//Remove self after spawning object.
        
    }
}
