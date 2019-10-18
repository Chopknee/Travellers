using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDungeonSpawner: MonoBehaviour {

    public GameObject prefabToSpawn;

    void Start () {
        Invoke("SpawnThing", 1);
    }

    void SpawnThing() {
        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            NetInstanceManager.CurrentManager.Instantiate(prefabToSpawn, false, transform.position, transform.rotation);
        }
        Destroy(gameObject);//Remove self after spawning object.
    }
}
