using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject skeletonPrefab;
    public AudioSource skeletonSpawnFX;
    bool spawned;
    
    void InitiateSpawn()
    {
        CookieFlipBook c = transform.GetComponentInChildren<CookieFlipBook>();
        c.StartCoroutine(c.SwitchCookie());
        skeletonSpawnFX = GetComponent<AudioSource>();
        Invoke("PlayAudio", .5f);
        Invoke("SpawnGroup", 3f);
    }

    void PlayAudio()
    {
        skeletonSpawnFX.Play();
    }
    void SpawnGroup()
    {
        //spawn here
        //NetInstanceManager.CurrentManager.Instantiate(< prefab gameobject reference >, false, position, rotation)
        GameObject o = Instantiate(skeletonPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !spawned)
        {
            spawned = true;
            InitiateSpawn();
        }
    }
}
