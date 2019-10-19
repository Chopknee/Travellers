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
    public float attackRadius = 5;
    public LayerMask layer_mask;
    public float distanceToCurrentTarget = 0;

    public float destinationSettingDelay;
    Timeout setDestTimeout;

    float sightRadSqd;
    float attackRadSqd;

    private void Awake () {
        agent = GetComponent<NavMeshAgent>();
        style.wielder = gameObject;
        setDestTimeout = new Timeout(destinationSettingDelay, true);
        sightRadSqd = sightRadius * sightRadius;
        attackRadSqd = attackRadius * attackRadius;
        if (GetComponent<Health>() != null) {
            GetComponent<Health>().OnHealthChanged += Damaged;
        }

    }

    private void Update () {
        if (setDestTimeout.Tick(Time.deltaTime)) {
            targets = Physics.OverlapSphere(transform.position + sightOffset * transform.forward, sightRadius, layer_mask);
            currentTarget = SelectClosestTarget(targets, currentTarget);
            if (currentTarget != null) {
                distanceToCurrentTarget = ( currentTarget.position - transform.position ).sqrMagnitude;
                if (( transform.position - currentTarget.position ).sqrMagnitude < Mathf.Pow(sightRadSqd, 2)) {
                    //chase player
                    agent.SetDestination(currentTarget.position);
                }
            }
            setDestTimeout.Reset();
            setDestTimeout.Start();
        }
        if (currentTarget != null) {
            distanceToCurrentTarget = ( currentTarget.transform.position - transform.position ).sqrMagnitude;

            if (distanceToCurrentTarget < attackRadSqd) {
                style.DoAttack();
            }
        }
    }

    void Damaged(float hp, GameObject damager) {
        currentTarget = damager.transform;
    }

    private void OnDestroy () {
        if (GetComponent<Health>() != null) {
            GetComponent<Health>().OnHealthChanged -= Damaged;
        }
    }

    public Transform SelectClosestTarget (Collider[] targets, Transform current) {
        //When no targets are in range, just keep the existing target. (This causes the enemy to continue following the last followed)
        if (targets.Length == 0) { return current; }

        Transform ct = current;
        //Automatically fill the a null target
        if (targets.Length > 0 && ct == null) {
            ct = targets[0].transform;
        }

        //Find the closest target and attack it.
        float cDist = ( ct.transform.position - transform.position ).sqrMagnitude;
        foreach (Collider c in targets) {
            float nDist = ( c.transform.position - transform.position ).sqrMagnitude;
            if (nDist < cDist) {
                ct = c.transform;
                cDist = nDist;
            }
        }
        return ct;
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sightOffset * transform.forward, sightRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
