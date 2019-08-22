using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRoof: MonoBehaviour {
    [SerializeField]
#pragma warning disable 0649
    private GameObject rootRoof;

    private void OnTriggerEnter ( Collider other ) {
        if (other.tag == "Player") {
            if (other.GetComponent<PhotonView>() != null && other.GetComponent<PhotonView>().IsMine || other.GetComponent<PhotonView>() == null) {
                //Hide the object
                //rootRoof.SetActive(false);
                rootRoof.GetComponent<Animator>().SetBool("Visible", false);
            }
        }
    }

    private void OnTriggerExit ( Collider other ) {
        if (other.tag == "Player") {
            if (other.GetComponent<PhotonView>()!=null && other.GetComponent<PhotonView>().IsMine || other.GetComponent<PhotonView>() == null) {
                //Show the object
                //rootRoof.SetActive(true);
                rootRoof.GetComponent<Animator>().SetBool("Visible", true);
            }
        }
    }
}
