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
    bool player;
    private Vector3 velocity;

    public Animator anim;

    AnimationClip lastClip;
    NavMeshAgent agent;
    Vector3 lastPos, nextPos;
    private void Start()
    {
        lastPos = transform.position;
        if (transform.CompareTag("Player"))
            player = true;

        if(player)
            anim = transform.GetComponent<Animator>();
        else
            anim = transform.GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    float smoothVel = 0;
    float lastSpd = 0;
    private void LateUpdate()
    {
        nextPos = transform.position;
        float lv = (nextPos - lastPos).sqrMagnitude / Time.fixedDeltaTime;
        float tmpSpeed = 100 * Mathf.SmoothDamp(lv, (float)System.Math.Round(lv, 1), ref smoothVel, .9f);
        if(!player) speed = Mathf.Lerp(lastSpd, tmpSpeed, .3f);
        lastPos = transform.position;
        lastSpd = tmpSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(player) speed = (float)System.Math.Round(GetComponent<PlayerMovement>().currentRunSpeed, 2);

        

        anim.SetFloat("Runspeed", speed);
        


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
