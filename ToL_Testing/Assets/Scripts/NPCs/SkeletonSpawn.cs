using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject skeletonPrefab;
    public AudioSource skeletonSpawnFX;

    // Start is called before the first frame update
    void Start()
    {
        CookieFlipBook c = transform.GetComponentInChildren<CookieFlipBook>();
        c.StartCoroutine(c.SwitchCookie());
        skeletonSpawnFX = GetComponent<AudioSource>();
        Invoke("PlayAudio", .5f);
        Invoke("Spawn", 3f);
    }
    void PlayAudio()
    {
        skeletonSpawnFX.Play();
    }
    void Spawn()
    {
        //spawn here
        //NetInstanceManager.CurrentManager.Instantiate(< prefab gameobject reference >, false, position, rotation)
        GameObject o = Instantiate(skeletonPrefab, spawnPoint.position, Quaternion.identity);
        

    }
}
