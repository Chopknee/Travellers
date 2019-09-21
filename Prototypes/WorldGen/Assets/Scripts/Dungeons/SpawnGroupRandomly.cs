using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGroupRandomly : EncounterSpawn {
    public override void SpawnEncounter () {
        //Randomly pick npcs
        foreach (Transform point in spawnPoints) {
            int ind = Mathf.FloorToInt(Random.value * npcs.Length);
            NetInstanceManager.CurrentManager.Instantiate(npcs[ind], false, point.position, point.rotation);
        }
    }
}
