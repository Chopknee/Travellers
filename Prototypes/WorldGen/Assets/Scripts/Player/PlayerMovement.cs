using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BaD.Modules.Control {

    public class PlayerMovement: MonoBehaviour {

        public LayerMask layer_mask;
        public Vector3 destinationPosition;
        Quaternion targetRotation;
        public GameObject arrow;
        public WaypointAnimations currentArrow;
        NavMeshAgent agent;
        public bool followingArrow;
        public GameObject particles;

        public float agentRemainingDist = 0;
        public bool pathfinding = false;

        private void Start () {
            destinationPosition = transform.position;
            agent = GetComponent<NavMeshAgent>();
        }
        private void Update () {
            //Normal navigation
            if (Input.GetButtonDown("Interact")) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Camera.main.farClipPlane, layer_mask)) {
                    string t = hit.collider.tag;
                    if (t == ( "Room" )) {
                        SetDestination(hit.point, true);
                    }
                }
            }

            if (pathfinding) {
                switch (agent.pathStatus) {
                    case NavMeshPathStatus.PathComplete:
                        pathfinding = false;
                        Debug.Log("Path found.");
                        break;
                    case NavMeshPathStatus.PathInvalid:
                        pathfinding = false;
                        Debug.Log("Path invalid.");
                        break;
                    case NavMeshPathStatus.PathPartial:
                        pathfinding = false;
                        Debug.Log("Path ended early.");
                        break;
                }
            }
            agentRemainingDist = agent.remainingDistance;
            if (currentArrow != null) {
                if (!agent.pathPending) {
                    if (agent.remainingDistance <= agent.stoppingDistance) {
                        if (!agent.hasPath || GetComponent<MovementAnimationController>().speed <= 0) {
                            Debug.Log("Finished navigating.");
                            KillArrow();
                            particles.GetComponent<ParticleSystem>().Play();
                        }
                    }
                }
            }


            if (currentArrow != null && followingArrow) {
                if (Vector3.Distance(transform.position, currentArrow.transform.position) < .1f) {
                    KillArrow();
                }
            }

        }

        private void OnDestroy () {
            if (currentArrow != null) {
                currentArrow.Die();
                currentArrow = null;
            }
        }
        private void OnDisable () {
            if (currentArrow != null) {
                currentArrow.Die();
                currentArrow = null;
            }
        }

        // location to move to, does it have an arrow with it
        public void SetDestination ( Vector3 dest, bool hasArrow ) {
            Debug.Log("<color=blue>Player destination set to " + dest + "</color>");

            Vector3 targetLookPoint = new Vector3(dest.x, transform.position.y, dest.z);

            targetRotation = Quaternion.LookRotation(targetLookPoint - transform.position);

            if (dest != null && targetRotation != null && dest != transform.position) {
                if (currentArrow != null) {
                    KillArrow();
                }

                agent.SetDestination(dest);
                pathfinding = true;
                if (hasArrow) {
                    CreateArrow(dest);
                }
            }
        }


        public void CreateArrow ( Vector3 dest ) {


            GameObject o = null;
            o = Instantiate(arrow, new Vector3(dest.x, 0, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));

            if (Physics.Raycast(dest + Vector3.up * 100, Vector3.down, out RaycastHit hit, 500, layer_mask)) {
                o.transform.position = hit.point + Vector3.up * 3.14f;
            }

            currentArrow = o.transform.Find("wp").GetComponent<WaypointAnimations>();

            followingArrow = true;

        }

        public void KillArrow () {
            if (currentArrow != null) {
                currentArrow.Die();
                currentArrow = null;
            }
        }
    }
}