using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementAnimationController : MonoBehaviour
{
    bool transitioning;
    public float transitionTime = 2;
    float fallSpeed = .02f;

    public enum state
    {
        Attack, Die
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

    float[] avgSpeedArray = new float[10];

    int indexOfAvg = 0;
    public float avgSpeed;
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
        float lv = (nextPos - lastPos).sqrMagnitude / Time.deltaTime;
        float tmpSpeed = 100 * Mathf.SmoothDamp(lv, (float)System.Math.Round(lv, 1), ref smoothVel, .5f);
        float roundSpeed = Mathf.RoundToInt(speed);

        speed = Mathf.Lerp(lastSpd, avgSpeed, .3f);
        lastPos = transform.position;
        lastSpd = tmpSpeed;


        speed = (float)System.Math.Round(speed, 2);

        
        
        avgSpeedArray[indexOfAvg] = speed;
        indexOfAvg = (indexOfAvg == avgSpeedArray.Length - 1) ? 0 : indexOfAvg + 1;
        avgSpeed = 0;
        foreach (float f in avgSpeedArray)
        {
            avgSpeed += f;
        }

        avgSpeed = avgSpeed / (float)avgSpeedArray.Length;

        anim.SetFloat("Runspeed", avgSpeed);


        SetAnimation();
    }


    Vector3 nMeshPos = Vector3.zero;

    // rope animation transition
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
        GetComponent<NPCSpawnInitialization>().Initialize();
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
