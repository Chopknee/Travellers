using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementAnimationController : MovementAnimationController
{
    bool transitioning;

    public new float transitionTime = 2;
    float fallSpeed = .01f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        transitioning = true;
        StartCoroutine("Transition", transitionTime);
    }

  

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (transitioning)
    //    {
    //        transform.position -= Vector3.up * fallSpeed;
    //    }
    //    else
    //    {

    //        speed = GetComponent<NavMeshAgent>().velocity.normalized.magnitude;

    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            anim.SetTrigger("Attack");
    //        }
    //        if (speed <= .8f)
    //        {
    //            nextState = state.Idle;
    //        }
    //        else if (speed > .8f)
    //        {
    //            nextState = state.Walk;
    //            if (speed > 3f)
    //            {
    //                nextState = state.Run;
    //            }
    //        }
    //        SetAnimation();
    //    }

    //}

    Vector3 nMeshPos = Vector3.zero;
    IEnumerator Transition(float t)
    {
        anim.SetBool("RopeClimbing", true);
        yield return new WaitForSeconds(t);
        NavMeshHit navMeshHit;
        Vector3 positionToCheck = gameObject.transform.position;
        NavMesh.SamplePosition(positionToCheck, out navMeshHit, 2, NavMesh.AllAreas);
        //nMeshPos = navMeshHit.position;

        yield return new WaitUntil(() => transform.position.y <= 0);
        Debug.Log("We gettin hot boys the floor is lava bb");
        anim.SetBool("RopeClimbing", false);
        transitioning = false;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nMeshPos, 1);
    }

    void SetAnimation()
    {
        AnimatorClipInfo[] clipInfo;
        clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        if (lastState == nextState) return;

        Debug.Log("Transitioning from " + lastState.ToString() + " to " + nextState.ToString());
        anim.SetTrigger(nextState.ToString());
        lastState = nextState;
    }
}
