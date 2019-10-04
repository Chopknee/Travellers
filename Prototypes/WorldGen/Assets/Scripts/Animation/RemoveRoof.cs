using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRoof: MonoBehaviour {
    [SerializeField]
#pragma warning disable 0649
    private GameObject rootRoof;

    [SerializeField]
#pragma warning disable 0649
    private AnimationCurve blendCurve;
    public float animationTime = 1;

    public string TransitionName = "HideAmount";

    public Animator baseAnimator;
    SmoothCount transition;

    public void Start () {
        
        if (baseAnimator == null) {
            baseAnimator = GetComponent<Animator>();
            if (baseAnimator == null) {
                //gameObject.SetActive(false);
                enabled = false;
            }
        }
        transition = new SmoothCount(blendCurve, 0, 1, animationTime);
    }

    float amount;

    public void Update () {
        if (transition.isRunning) {
            amount = transition.DriveForward(Time.deltaTime);
            baseAnimator.SetFloat(TransitionName, amount);
        }
    }

    private void OnTriggerEnter ( Collider other ) {
        if (other.tag == "Player") {
            //if (other.GetComponent<PhotonView>() != null && other.GetComponent<PhotonView>().IsMine || other.GetComponent<PhotonView>() == null) {
            transition.Start(true);
            Debug.Log("Player entered!");
            //}
        }
    }

    private void OnTriggerExit ( Collider other ) {
        if (other.tag == "Player") {
            //if (other.GetComponent<PhotonView>()!=null && other.GetComponent<PhotonView>().IsMine || other.GetComponent<PhotonView>() == null) {

            transition.Start(false);
            Debug.Log("Player exited!");
            //}
        }
    }
}
