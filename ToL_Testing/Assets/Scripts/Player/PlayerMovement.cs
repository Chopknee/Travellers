using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement : MonoBehaviour
{
    public Vector3 destinationPosition;
    Quaternion targetRotation;
    Rigidbody rb;
    public GameObject arrow;
    public float walkSpeed = 1;
    public float turnSpeed = .5f;
    public float maxSpeed = 10;
    public float currentRunSpeed;
    public float extraRotationSpeed = 5f;
    List<WaypointAnimations> arrowsInGame = new List<WaypointAnimations>();
    NavMeshAgent agent;

    Vector3 lastPos, nextPos;
    private void Start()
    {
        lastPos = transform.position;
        destinationPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    float smoothVel = 0;
    float lastSpd = 0;
    private void LateUpdate()
    {
        nextPos = transform.position;
        float lv = (nextPos - lastPos).sqrMagnitude / Time.fixedDeltaTime;
        float tmpSpeed = 100 * Mathf.SmoothDamp(lv, (float)System.Math.Round(lv, 1), ref smoothVel, .9f);
        currentRunSpeed = Mathf.Lerp(lastSpd, tmpSpeed, .3f);
        lastPos = transform.position;
        lastSpd = tmpSpeed;
    }
    private void Update()
    {
        //Vector3 lookrotation = agent.steeringTarget - transform.position;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);





        if (arrowsInGame.Count >= 2)
        {
            Destroy(arrowsInGame[1]);
        }



        int layer_mask = LayerMask.GetMask("Default");

        if (Input.GetMouseButtonDown(1) || forceArrowGeneration)
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitDist = 0;
            //if(playerPlane.Raycast(ray, out hitDist))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layer_mask))
                {
                    string t = hit.collider.tag;
                    if (t == ("Room"))
                    {

                        Debug.Log("Found a room...?");
                        Vector3 p = hit.collider.transform.root.transform.position;
                        Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                        targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                        destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z); //ray.GetPoint(hitDist);

                        // transform.LookAt(directionOfTarget);

                        if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
                        {

                            currentRunSpeed += .2f;
                            GetComponent<NavMeshAgent>().SetDestination(destinationPosition);
                        }
                        CreateArrow(destinationPosition);

                    }
                }
            }
        }



    }
    bool forceArrowGeneration;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PortalOut"))
        {
            Debug.LogError("----------------------Exited----------------------");
        }
    }

    private void OnDestroy()
    {
        foreach (WaypointAnimations o in arrowsInGame)
        {
            if (o != null)
                Destroy(o.transform.root.gameObject);
        }
    }
    private void OnDisable()
    {
        foreach (WaypointAnimations o in arrowsInGame)
        {
            if (o != null)
                Destroy(o.transform.root.gameObject);
        }
    }
    WaypointAnimations currentArrow;
    float lastTime;
    void CreateArrow(Vector3 dest)
    {
        if (currentArrow != null)
        {
            arrowsInGame.Remove(currentArrow);
            Destroy(currentArrow.transform.root.gameObject);
        }

        float next = 50f;
        lastTime = Time.time + next;


        GameObject o = null;
        o = Instantiate(arrow, new Vector3(dest.x, 0, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));
        currentArrow = o.transform.Find("wp").GetComponent<WaypointAnimations>();
        arrowsInGame.Add(currentArrow);

        Invoke("KillArrow", 4f);
        KillArrow();

    }

    void KillArrow()
    {
        currentArrow.Die();
    }
}
