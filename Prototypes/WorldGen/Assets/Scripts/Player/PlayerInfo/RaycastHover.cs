using BaD.Modules.Control;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaycastHover: MonoBehaviour {
    public LayerMask layer_mask;

    Vector3 destinationPosition;
    Quaternion targetRotation;
    NavMeshAgent agent;
    PlayerMovement pm;
    Transform target;
    bool chase;
    PhotonView view;
    CombatController cc;

    private void Awake () {
        agent = GetComponent<NavMeshAgent>();
        pm = GetComponent<PlayerMovement>();
        view = GetComponent<PhotonView>();
        cc = GetComponent<CombatController>();
    }

    private void Update () {
        if (view.IsMine) {
            if (Input.GetButtonDown("Attack")) {

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Camera.main.farClipPlane, layer_mask)) {

                    if (hit.collider.tag == ( "Enemy" )) {
                        Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                        targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                        destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                        pm.KillArrow();
                        agent.isStopped = true;
                        agent.isStopped = false;

                        transform.LookAt(directionOfTarget);
                        agent.SetDestination(destinationPosition);
                        target = hit.transform;
                        chase = true;
                    }
                } else {
                    //If we don't hit something, stop chasing?
                    target = null;
                }
            }

            if (target != null) {
                //The distance to our target???
                if (target.GetComponentInParent<NPC>() != null) {
                    if (( transform.position - target.position ).sqrMagnitude < target.GetComponentInParent<NPC>().attackRadSquared) {
                        //can do an attack now.
                        cc.canAttack = true;
                    }
                } else {
                    cc.canAttack = false;
                }
            } else {
                cc.canAttack = false;
            }
        }
    }
}
