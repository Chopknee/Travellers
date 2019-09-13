using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterSpawn : MonoBehaviour {

    public Transform[] spawnPoints;
    public GameObject[] npcs;

    public abstract void SpawnEncounter ();
}
