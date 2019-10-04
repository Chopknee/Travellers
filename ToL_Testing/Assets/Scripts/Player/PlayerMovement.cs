using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement : MonoBehaviour
{
    public LayerMask layer_mask;
    public Vector3 destinationPosition;
    Quaternion targetRotation;
    public GameObject arrow;
    public WaypointAnimations currentArrow;
    NavMeshAgent agent;
    public bool followingArrow;
    public GameObject particles;


    private void Start()
    {
        destinationPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Camera.main.farClipPlane, layer_mask))
            {
                string t = hit.collider.tag;
                if (t == ("Room"))
                {
                    Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    SetDestination(destinationPosition, true);
                }
            }
        }

        if (currentArrow != null)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || GetComponent<MovementAnimationController>().speed <= 0)
                    {
                        KillArrow();
                        particles.GetComponent<ParticleSystem>().Play();
                    }
                }
            }

            if (!agent.pathPending && !agent.hasPath)
            {
                timeSincePathfinding += .1f;
            }
            else timeSincePathfinding = 0;

            if (timeSincePathfinding >= 5f)
            {
                KillArrow();
            }
        }


        if (currentArrow != null && followingArrow)
        {
            if (Vector3.Distance(transform.position, currentArrow.transform.position) < .1f)
            {
                KillArrow();
            }
        }

    }

    float timeSincePathfinding = 0;

    private void OnDestroy()
    {
        if (currentArrow != null)
        {
            currentArrow.Die();
            currentArrow = null;
        }
    }
    private void OnDisable()
    {
        if (currentArrow != null)
        {
            currentArrow.Die();
            currentArrow = null;
        }
    }

    public void SetDestination(Vector3 dest, bool hasArrow) // location to move to, does it have an arrow with it
    {
        if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
        {
            if (currentArrow != null)
            {
                KillArrow();
            }

            GetComponent<NavMeshAgent>().SetDestination(destinationPosition);

            if (hasArrow)
                CreateArrow(dest);
        }
    }


    public void CreateArrow(Vector3 dest)
    {


        GameObject o = null;
        o = Instantiate(arrow, new Vector3(dest.x, 0, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));

        if(Physics.Raycast(dest, Vector3.down, out RaycastHit hit, 500, layer_mask))
        {
            o.transform.position = hit.point + Vector3.up * 3.14f;
        }

        currentArrow = o.transform.Find("wp").GetComponent<WaypointAnimations>();

        followingArrow = true;

    }

    public void KillArrow()
    {
        if (currentArrow != null)
        {
            currentArrow.Die();
            currentArrow = null;
        }
    }
}
