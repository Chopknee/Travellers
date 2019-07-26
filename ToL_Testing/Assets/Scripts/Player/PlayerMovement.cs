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
        int layer_mask = LayerMask.GetMask("Default");

        if (Input.GetMouseButtonDown(0))
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitDist = 0;
            //if(playerPlane.Raycast(ray, out hitDist))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, layer_mask))
                {
                    destinationPosition = hit.point; //ray.GetPoint(hitDist);
                    targetRotation = Quaternion.LookRotation(destinationPosition - transform.position);
                    StartCoroutine(CreateArrow(destinationPosition));
                }
            }
        }
        if(destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
        {
            GetComponent<NavMeshAgent>().SetDestination(destinationPosition);
        }

        
        
    }

    IEnumerator CreateArrow(Vector3 dest)
    {
        GameObject o = Instantiate(arrow, dest + new Vector3(0, 1, 0), Quaternion.Euler(new Vector3(-90,0,0)));
        arrowsInGame = GameObject.FindGameObjectsWithTag("DestinationArrowTmp");
        if(arrowsInGame.Length < 1)
        {
            Destroy(arrowsInGame[0]);
        }
        yield return new WaitForSeconds(.1f);
        
        yield return new WaitUntil(() => Vector3.Distance(transform.position, dest) <= 2 || Input.GetMouseButtonDown(0));
        Destroy(o);
    }
}
