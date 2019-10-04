using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyCamera : MonoBehaviour
{
    Transform player;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine == false) return;

        GetComponent<Camera>().enabled = true;

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (g.GetComponent<PhotonView>().IsMine)
            {
                player = g.transform;
            }
        }

    }

    private void LateUpdate()
    {
        transform.LookAt(player);
    }
}
