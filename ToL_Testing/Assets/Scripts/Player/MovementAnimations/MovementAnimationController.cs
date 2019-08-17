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
        if (transform.CompareTag("Player"))
            anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        else
            anim = transform.GetChild(0).GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if(transform.CompareTag("Player")) speed = (float)System.Math.Round(GetComponent<PlayerMovement>().currentRunSpeed, 2);
        anim.SetFloat("Runspeed", speed);

        //anim.speed = speed;
        //if (agent.isStopped || speed < .1f)
        //{
        //    nextState = state.Idle;
        //}

        //if(speed > .1f)
        //{
        //    nextState = state.Walk;
        //}


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
