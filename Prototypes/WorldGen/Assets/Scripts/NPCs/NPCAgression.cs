using BaD.Modules.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAgression: MonoBehaviour {

    public Collider[] targets;
    public Transform currentTarget;
    public NavMeshAgent agent;
    public AttackStyle style;
    public float sightRadius = 5, sightOffset = 2;
    public LayerMask layer_mask;
    public float distanceToCurrentTarget = 0;

    private void Awake () {
        agent = GetComponent<NavMeshAgent>();
        style.wielder = gameObject;
    }

    private void Update () {
        targets = Physics.OverlapSphere(transform.position + sightOffset * transform.forward, sightRadius, layer_mask);

        if (currentTarget != null) {
            distanceToCurrentTarget = ( currentTarget.position - transform.position ).sqrMagnitude;
            if (( transform.position - currentTarget.position ).sqrMagnitude < Mathf.Pow(sightRadius, 2)) {
                //chase player
                agent.SetDestination(currentTarget.position);
            }
            style.DoAttack();
        }
        FindTarget();
    }

    public void FindTarget () {
        if (targets.Length > 0 && currentTarget == null) {
            currentTarget = targets[0].transform;
        }

        foreach (Collider c in targets) {
            if (( c.transform.position - transform.position ).sqrMagnitude < distanceToCurrentTarget) {
                currentTarget = c.transform;
            }
        }
    }

    private void OnDrawGizmos () {
        Gizmos.color = new Color(255, 0, 0, .2f);
        Gizmos.DrawSphere(transform.position, sightOffset);
    }
}
