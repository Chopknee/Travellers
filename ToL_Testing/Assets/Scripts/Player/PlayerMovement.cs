using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement : MonoBehaviour
{
    NavMeshAgent agent;

    public string roomTagForPathfinding = "Room";

    public Vector3 destinationPosition;

    Quaternion targetRotation;

    GameObject[] arrowsInGame;


    public LayerMask layer_mask;

    public GameObject arrow;

    private void Start()
    {
        destinationPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
    }

    float smoothVel = 0;
    float lastSpd = 0;

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layer_mask))
            {
                string t = hit.collider.tag;
                if (t == (roomTagForPathfinding))
                {
                    Vector3 p = hit.collider.transform.root.transform.position;
                    Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z); 
                    
                    if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
                    {
                        agent.SetDestination(destinationPosition);

                        CreateArrow(destinationPosition);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PortalOut"))
        {
            Debug.LogError("----------------------Exited----------------------");
        }

        if (other.CompareTag("DestinationArrowTmp"))
        {
            if (arrow == other.gameObject)
                KillArrow();
        }
    }

    private void OnDrawGizmos()
    {
        if (arrow != null)
        {
            Gizmos.color = new Color(0, 255, 0, .2f);
            Gizmos.DrawWireSphere(arrow.transform.position, arrow.GetComponent<SphereCollider>().radius);

        }
    }


    float lastTime;
    void CreateArrow(Vector3 dest)
    {
        float next = 50f;
        lastTime = Time.time + next;

        arrow = Instantiate(arrow, new Vector3(dest.x, 0, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));

        //yield return new WaitForSeconds(.1f);

        //yield return new WaitUntil(() => Vector3.Distance(transform.position, dest) <= 2);

    }

    void KillArrow()
    {
        if (arrow != null)
        {
            arrow.transform.Find("wp").GetComponent<WaypointAnimations>().Die();
            arrow = null;
        }
    }
}
