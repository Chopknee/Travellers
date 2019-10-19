using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaycastHover : MonoBehaviour
{
    public LayerMask layer_mask;

    Vector3 destinationPosition;
    Quaternion targetRotation;
    NavMeshAgent agent;
    PlayerMovement pm;
    Transform target;
    bool chase;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layer_mask))
        {
            string t = hit.collider.tag;
            if (t == ("Enemy"))
            {
                Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                if (Input.GetButtonDown("Attack"))
                {
                    //pm.KillArrow();
                    agent.isStopped = true;
                    agent.isStopped = false;
                    transform.LookAt(directionOfTarget);
                    agent.SetDestination(destinationPosition);
                    target = hit.transform;
                    chase = true;
                }
            }
        }

        if (chase && target != null)
        {
            agent.SetDestination(destinationPosition);
            if (Input.GetButtonDown("Interact"))
            {
                chase = false;
            }
        }
    }
}
