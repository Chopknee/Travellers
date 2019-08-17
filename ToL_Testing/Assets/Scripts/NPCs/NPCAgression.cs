using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgression : MonoBehaviour
{

    public GameObject[] targets;
    public Transform currentTarget;
    public NavMeshAgent agent;
    public float sightDistance = 5;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) < sightDistance)
        {
            //chase player
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            FindTarget();
        }
    }

    public void FindTarget()
    {
        Debug.Log("Finding target..");
        int r = Random.Range(0, targets.Length);
        if (targets[r] != null)
            currentTarget = targets[r].transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, sightDistance);
    }
}
