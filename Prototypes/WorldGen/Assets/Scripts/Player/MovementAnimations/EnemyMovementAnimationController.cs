using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementAnimationController: MovementAnimationController {
    public float transitionTime = 2;

    private void Start () {
        anim = GetComponent<Animator>();
        StartCoroutine("Transition", transitionTime);
    }

    Vector3 nMeshPos = Vector3.zero;
    IEnumerator Transition ( float t ) {
        anim.SetBool("RopeClimbing", true);
        yield return new WaitForSeconds(t);
        NavMeshHit navMeshHit;
        Vector3 positionToCheck = gameObject.transform.position;
        NavMesh.SamplePosition(positionToCheck, out navMeshHit, 2, NavMesh.AllAreas);
        //nMeshPos = navMeshHit.position;

        yield return new WaitUntil(() => transform.position.y <= 0);
        Debug.Log("We gettin hot boys the floor is lava bb");
        anim.SetBool("RopeClimbing", false);
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nMeshPos, 1);
    }

    void SetAnimation () {
        AnimatorClipInfo[] clipInfo;
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        if (lastState == nextState) return;

        Debug.Log("Transitioning from " + lastState.ToString() + " to " + nextState.ToString());
        anim.SetTrigger(nextState.ToString());
        lastState = nextState;
    }
}
