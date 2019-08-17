using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementAnimationController : MonoBehaviour
{
    public enum state
    {
        Idle, Walk, Run, Attack, Die
    }

    public state lastState, nextState;

    public float speed;
    private Vector3 velocity;

    public Animator anim;

    AnimationClip lastClip;
    NavMeshAgent agent;

    private void Start()
    {
        anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        speed = agent.velocity.normalized.magnitude;

        if (agent.isStopped || agent.velocity.magnitude < .9f)
        {
            nextState = state.Idle;
        }
        else nextState = state.Walk;
        
        SetAnimation();
    }


    void SetAnimation()
    {

        if (lastState == nextState) return;
        

        Debug.Log("Transitioning from " + lastState.ToString() + " to " + nextState.ToString());
        anim.SetTrigger(nextState.ToString());
        lastState = nextState;
    }
}
