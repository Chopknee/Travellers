using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EncounterSpawn))]
public class SkeletonSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject skeletonPrefab;
    public AudioSource skeletonSpawnFX;
    bool spawned;
    
    void InitiateSpawn() {
        CookieFlipBook c = transform.GetComponentInChildren<CookieFlipBook>();
        c.StartCoroutine(c.SwitchCookie());
        skeletonSpawnFX = GetComponent<AudioSource>();
        Invoke("PlayAudio", .5f);
        Invoke("SpawnGroup", 3f);
    }

    void PlayAudio() {
        skeletonSpawnFX.Play();
    }

    void SpawnGroup() {
        //Only trigger the spawn if we are the master
        if (NetInstanceManager.CurrentManager.isInstanceMaster) {
            //spawn here
            //NetInstanceManager.CurrentManager.Instantiate(< prefab gameobject reference >, false, position, rotation)
            //GameObject o = Instantiate(skeletonPrefab, spawnPoint.position, Quaternion.identity);
            if (GetComponent(typeof(EncounterSpawn)) != null) {
                (GetComponent(typeof(EncounterSpawn)) as EncounterSpawn).SpawnEncounter();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !spawned) {
            spawned = true;
            InitiateSpawn();
        }
    }
}
