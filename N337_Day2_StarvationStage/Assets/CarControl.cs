using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CarControl : MonoBehaviourPun
{
    public float acceleration = 10, maxSpeed = 10, turnRadius = 4;

    public GameObject leftWheel, rightWheel;

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
            GetComponent<Rigidbody>().AddForce(Input.GetAxis("Vertical") * transform.forward * acceleration);

        leftWheel.transform.Rotate(transform.up * Input.GetAxis("Horizontal") * turnRadius);
        rightWheel.transform.Rotate(transform.up * Input.GetAxis("Horizontal") * turnRadius);
    }
}
