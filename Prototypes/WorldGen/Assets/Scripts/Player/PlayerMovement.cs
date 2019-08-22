using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//I fixed your player script because the old one was breaking too often
public class PlayerMovement : MonoBehaviour {

    public Vector3 destinationPosition;
    Quaternion targetRotation;
    public GameObject arrowPrefab;
    public float currentRunSpeed;
    public LayerMask pathfindLayermask;

    GameObject currentArrow;
    NavMeshAgent agent;


    private void Start() {
        destinationPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (Input.GetButtonDown("Interact")) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Camera.main.farClipPlane, pathfindLayermask)) {
                string t = hit.collider.tag;
                if (t == ( "Room" )) {
                    Vector3 p = hit.collider.transform.root.transform.position;
                    Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position) {
                        //Successfully found a target, maybe?
                        currentRunSpeed += .2f;
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

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PortalOut")) {
            //Specifically for exiting an instance
            Debug.LogError("----------------------Exited----------------------");
        }
    }
}
