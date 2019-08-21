using BaD.Modules.Terrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementAnimationController : MonoBehaviour
{
    bool transitioning;
    public float transitionTime = 2;
    float fallSpeed = .02f;
    public float speedAnimationRatio = 2;

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


        anim = transform.GetComponent<Animator>();

        if (player)
        {
        }
        else
        {
            transitioning = true;
            anim.SetBool("RopeClimbing", true);
            StartCoroutine("Transition", transitionTime);
        }



        agent = GetComponent<NavMeshAgent>();
    }


    float smoothVel = 0;
    float lastSpd = 0;
    private void LateUpdate()
    {
        if (transitioning)
        {
            transform.position -= Vector3.up * fallSpeed;
            return;
        }
        nextPos = transform.position;
        float lv = ( nextPos - lastPos ).sqrMagnitude / Time.deltaTime;
        float tmpSpeed = 100 * Mathf.SmoothDamp(lv, (float) System.Math.Round(lv, 1), ref smoothVel, .9f);
        if (!player) speed = Mathf.Lerp(lastSpd, tmpSpeed, .3f);
        lastPos = transform.position;
        lastSpd = tmpSpeed;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transitioning)
        {
            return;
        }

        speed = agent.velocity.magnitude * speedAnimationRatio;

        anim.SetFloat("Runspeed", speed);

        SetAnimation();
    }


    Vector3 nMeshPos = Vector3.zero;
    IEnumerator Transition(float t)
    {
        
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


    void SetAnimation()
    {

        if (lastState == nextState) return;


        Debug.Log("Transitioning from " + lastState.ToString() + " to " + nextState.ToString());
        anim.SetTrigger(nextState.ToString());
        lastState = nextState;
    }
}
