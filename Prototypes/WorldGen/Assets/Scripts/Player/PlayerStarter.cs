using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStarter : MonoBehaviour {

    public LayerMask navigationMask;

    private void Start () {
        Invoke("FixAltitude", 1);
        //FixAltitude();
    }

    private void OnEnable () {
        FixAltitude();
    }

    public void FixAltitude() {
        PhotonView pv = GetComponent<PhotonView>();
        NavMeshAgent nma = GetComponent<NavMeshAgent>();
        if (pv == null) { Debug.Log("Could not run network player activation script, missing photon view component!"); return; }
        if (nma == null) { Debug.Log("Could not run network player activation script, missing nav mesh agent component!"); return; }
        if (pv.IsMine) {
            //Raycast to the floor!
            nma.enabled = false;
            if (Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, out RaycastHit hit, Mathf.Infinity, navigationMask)) {
                transform.position = hit.point;
                nma.enabled = true;
            } else {
                Debug.Log("Player could not find valid navigation destination under player's position.");
            }
        }
    }
}
