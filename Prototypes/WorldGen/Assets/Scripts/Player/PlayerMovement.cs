using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement: MonoBehaviour {
    public Vector3 destinationPosition;
    Quaternion targetRotation;
    Rigidbody rb;
    public GameObject pointerPrefab;
    public float walkSpeed = 1;
    public float turnSpeed = .5f;
    public float maxSpeed = 10;
    GameObject[] arrowsInGame;
    private GameObject pointerInstance;
    public LayerMask dungeonFloorsLayerMask;
    NavMeshAgent agent;

    private void Start () {
        destinationPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        pointerInstance = Instantiate(pointerPrefab);
        pointerInstance.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        pointerInstance.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
    }
    private void FixedUpdate () {

        if (Input.GetMouseButtonDown(1) || forceArrowGeneration) {

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, dungeonFloorsLayerMask)) {
                if (hit.collider.transform.root.CompareTag("Room")) {
                    Vector3 p = hit.collider.transform.root.transform.position;
                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z); //ray.GetPoint(hitDist);
                    targetRotation = Quaternion.LookRotation(destinationPosition - transform.position);
                }
            }
        }

        if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position) {
            //I guess this is a found path situation.
            agent.SetDestination(destinationPosition);
            pointerInstance.transform.position = new Vector3(destinationPosition.x, 2, destinationPosition.z);
            pointerInstance.SetActive(true);
        }

        //Situation for ending travel.
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                pointerInstance.SetActive(false);
            }
        }

    }
    bool forceArrowGeneration;

    private void OnTriggerEnter ( Collider other ) {
        if (other.CompareTag("PortalOut")) {
            DungeonManager.CurrentInstance.ExitInstance();
            Debug.Log("----------------------Exited Instance----------------------");
        }
    }

    private void OnDisable () {
        pointerInstance.SetActive(false);
    }
}
