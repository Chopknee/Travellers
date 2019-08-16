using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementAnimationController : MovementAnimationController
{
    // Update is called once per frame
    void FixedUpdate()
    {
        speed = GetComponent<NavMeshAgent>().velocity.normalized.magnitude;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Attack");
        }
        if (speed <= .8f)
        {
            nextState = state.Idle;
        }
        else if (speed > .8f)
        {
            nextState = state.Walk;
            if (speed > 3f)
            {
                nextState = state.Run;
            }
        }
        SetAnimation();

        
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
