using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace BaD.Modules.Terrain {

    public class Player: MonoBehaviourPunCallbacks {

        public Vector3 destinationPosition;
        Quaternion targetRotation;
        public GameObject arrowPrefab;
        public LayerMask pathfindLayermask;

        GameObject currentArrow;
        NavMeshAgent agent;

        public PlayerData Data;


        private void Start () {
            Data = new PlayerData(gameObject);
            destinationPosition = transform.position;
            agent = GetComponent<NavMeshAgent>();
        }

        float smoothVel = 0;
        float lastSpd = 0;

        private void LateUpdate () {
            //currentRunSpeed = agent.velocity.magnitude;
        }

        private void Update () {
            if (Input.GetButtonDown("Interact")) {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Camera.main.farClipPlane, pathfindLayermask)) {
                    string t = hit.collider.tag;
                    if (t == ( "Map" )) {
                        Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                        targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                        destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                        if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position) {
                            //Successfully found a target, maybe?
                            agent.SetDestination(destinationPosition);

                            //Since we have a new destination, lets set up a new arrow instance.
                            if (currentArrow != null) {
                                currentArrow.transform.Find("wp").GetComponent<WaypointAnimations>().Die();
                                currentArrow = null;
                            }
                            currentArrow = Instantiate(arrowPrefab, new Vector3(destinationPosition.x, 100, destinationPosition.z), Quaternion.Euler(new Vector3(-90, 0, 0)));
                            //Reposition the thing so it is above the current ground level
                            currentArrow.transform.position = hit.point + new Vector3(0, 3.827f, 0);
                        }
                    }
                }
            }

            //For clearing old arrow instances.
            if (currentArrow != null) {
                if (!agent.pathPending) {
                    if (agent.remainingDistance <= agent.stoppingDistance) {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                            currentArrow.transform.Find("wp").GetComponent<WaypointAnimations>().Die();
                        }
                    }
                }
            }
        }

        public void SetDestination ( Vector3 dest ) {
            Vector3 directionOfTarget = new Vector3(dest.x, transform.position.y, dest.z);

            targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
            destinationPosition = new Vector3(dest.x, transform.position.y, dest.z);

            if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position) {
                //Successfully found a target, maybe?
                agent.SetDestination(destinationPosition);

                //Since we have a new destination, lets set up a new arrow instance.
                if (currentArrow != null) {
                    currentArrow.transform.Find("wp").GetComponent<WaypointAnimations>().Die();
                    currentArrow = null;
                }
                currentArrow = Instantiate(arrowPrefab, new Vector3(destinationPosition.x, 100, destinationPosition.z), Quaternion.Euler(new Vector3(-90, 0, 0)));
                //Reposition the thing so it is above the current ground level
                currentArrow.transform.position = dest + new Vector3(0, 3.827f, 0);
            }
        }
    }
}
