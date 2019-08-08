using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thriller_follow : MonoBehaviour
{
    public Vector3 offset = new Vector3(-5, 5, 0);
    Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Invoke("thriller", 1f);
    }

    public void LateUpdate()
    {
        transform.position = player.position + offset;
        transform.LookAt(player);
    }

    void thriller()
    {
        transform.GetChild(0).GetComponent<AudioSource>().Play();
    }
}
