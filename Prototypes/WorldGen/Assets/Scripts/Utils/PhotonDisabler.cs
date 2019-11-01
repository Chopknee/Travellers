using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonDisabler: MonoBehaviour {

    PhotonView view;
    public MonoBehaviour[] BehavioursToDisable;
    bool lastIsMine = true;

    void Start () {
        view = GetComponent<PhotonView>();
    }

    void Update () {
        if (view.IsMine != lastIsMine) {
            foreach (MonoBehaviour beh in BehavioursToDisable) {
                beh.enabled = view.IsMine;
            }
            lastIsMine = view.IsMine;
        }
    }
}
