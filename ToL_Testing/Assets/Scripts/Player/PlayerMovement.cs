//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//public class PlayerMovement : MonoBehaviour
//{
//    NavMeshAgent agent;

//    public string roomTagForPathfinding = "Room";

//    public Vector3 destinationPosition;

//    Quaternion targetRotation;

//    GameObject[] arrowsInGame;


//    public LayerMask layer_mask;

//    public GameObject arrow;

//    private void Start()
//    {
//        destinationPosition = transform.position;
//        agent = GetComponent<NavMeshAgent>();
//    }

//    float smoothVel = 0;
//    float lastSpd = 0;

//    private void Update()
//    {
//        if (Input.GetButtonDown("Interact"))
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layer_mask))
//            {
//                string t = hit.collider.tag;
//                if (t == (roomTagForPathfinding))
//                {
//                    Vector3 p = hit.collider.transform.root.transform.position;
//                    Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

//                    targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
//                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z); 

//                    if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
//                    {
//                        agent.SetDestination(destinationPosition);

//                        CreateArrow(destinationPosition);
//                    }
//                }
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("PortalOut"))
//        {
//            Debug.LogError("----------------------Exited----------------------");
//        }

//        if (other.CompareTag("DestinationArrowTmp"))
//        {
//            if (arrow == other.gameObject)
//                KillArrow();
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        if (arrow != null)
//        {
//            Gizmos.color = new Color(0, 255, 0, .2f);
//            Gizmos.DrawWireSphere(arrow.transform.position, arrow.GetComponent<SphereCollider>().radius);

//        }
//    }


//    float lastTime;
//    void CreateArrow(Vector3 dest)
//    {
//        float next = 50f;
//        lastTime = Time.time + next;

//        arrow = Instantiate(arrow, new Vector3(dest.x, transform.position.y + 2, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));

//        //yield return new WaitForSeconds(.1f);

//        //yield return new WaitUntil(() => Vector3.Distance(transform.position, dest) <= 2);

//    }

//    void KillArrow()
//    {
//        if (arrow != null)
//        {
//            arrow.transform.Find("wp").GetComponent<WaypointAnimations>().Die();
//            arrow = null;
//        }
//    }
//}


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

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layer_mask))
            {
                string t = hit.collider.tag;
                if (t == ("Room"))
                {
                    Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    
                    if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
                    {
                        if(currentArrow != null)
                        {
                            KillArrow();
                        }
                        GetComponent<NavMeshAgent>().SetDestination(destinationPosition);
                        CreateArrow(destinationPosition);
                    }
                }
            }
        }

        if(currentArrow != null)
        {
            if (!agent.pathPending)
            {
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    if(!agent.hasPath || GetComponent<MovementAnimationController>().speed <= 0)
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


        if(currentArrow != null && followingArrow)
        {
            if(Vector3.Distance(transform.position, currentArrow.transform.position) < .1f)
            {
                KillArrow();
            }
        }

    }

    float timeSincePathfinding = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PortalOut"))
        {
            Debug.LogError("----------------------Exited----------------------");
        }
    }

    private void OnDestroy()
    {
        if(currentArrow != null)
        {
            currentArrow.Die();
            currentArrow = null;
        }
    }
    private void OnDisable()
    {
        if(currentArrow != null)
        {
            currentArrow.Die();
            currentArrow = null;
        }
    }
    
    
    void CreateArrow(Vector3 dest)
    {


        GameObject o = null;
        o = Instantiate(arrow, new Vector3(dest.x, 0, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));
        currentArrow = o.transform.Find("wp").GetComponent<WaypointAnimations>();

        followingArrow = true;

    }

    void KillArrow()
    {
        currentArrow.Die();
        currentArrow = null;
    }
}
