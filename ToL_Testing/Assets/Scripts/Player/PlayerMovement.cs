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
    GameObject[] arrowsInGame;
    private void Start()
    {
        destinationPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        arrowsInGame = GameObject.FindGameObjectsWithTag("DestinationArrowTmp");

        if (arrowsInGame.Length >= 2)
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
                    if (hit.collider.transform.root.CompareTag("Room"))
                    {
                        Vector3 p = hit.collider.transform.root.transform.position;
                        destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z); //ray.GetPoint(hitDist);
                        targetRotation = Quaternion.LookRotation(destinationPosition - transform.position);


                        StartCoroutine(CreateArrow(destinationPosition));
                        
                    }
                }
            }
        }
        if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
        {
            GetComponent<NavMeshAgent>().SetDestination(destinationPosition);
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
    float lastTime;
    IEnumerator CreateArrow(Vector3 dest)
    {
        float next = 50f;
        lastTime = Time.time + next;
        arrowsInGame = GameObject.FindGameObjectsWithTag("DestinationArrowTmp");

        GameObject o = null;
        { o = Instantiate(arrow, new Vector3(dest.x, 2, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0))); }
        
        yield return new WaitForSeconds(.1f);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, dest) <= 2 || Input.GetMouseButtonDown(1));


        Destroy(o);
        

    }
}
