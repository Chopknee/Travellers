using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerEnabled : MonoBehaviour, IPunObservable {

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool lastActive = false;

    public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info ) {
        if (stream.IsWriting) {//Only send the data if the instance is miine????
            stream.SendNext(gameObject.activeInHierarchy);
        } else {
            bool st = (bool) stream.ReceiveNext();
            if (st != lastActive) {
                gameObject.SetActive(st);
                lastActive = st;
            }

        }
    }
}
