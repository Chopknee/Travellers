using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementAnimationController: MonoBehaviour {
    bool transitioning;
    public float transitionTime = 2;
    float fallSpeed = .02f;

    public enum state {
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

    public float speedMultiplier = 1.7f;

    private void Start () {
        lastPos = transform.position;
        if (transform.CompareTag("Player"))
            player = true;


        anim = transform.GetComponent<Animator>();

        if (player) {
        } else {
            transitioning = true;
            anim.SetBool("RopeClimbing", true);
            StartCoroutine("Transition", transitionTime);
        }



        agent = GetComponent<NavMeshAgent>();
    }


    float smoothVel = 0;
    float lastSpd = 0;

    float[] smoothArray = new float[10];
    int pos = 0;

    private void LateUpdate () {
        if (transitioning) {
            transform.position -= Vector3.up * fallSpeed;
            return;
        }
        nextPos = transform.position;
        float lv = ( nextPos - lastPos ).magnitude / Time.deltaTime;
        float tmpSpeed = 100 * Mathf.SmoothDamp(lv, (float) System.Math.Round(lv, 1), ref smoothVel, .5f);
        speed = Mathf.Lerp(lastSpd, tmpSpeed, .3f);
        lastPos = transform.position;
        lastSpd = tmpSpeed;

        smoothArray[pos] = speed;
        pos = ( pos == smoothArray.Length - 1 ) ? 0 : pos + 1;
        float avgSpeed = 0;
        foreach (float val in smoothArray) {
            avgSpeed += val;
        }
        avgSpeed = avgSpeed / (float) smoothArray.Length;

        avgSpeed = (float) System.Math.Round(avgSpeed, 2);

        anim.SetFloat("Runspeed", avgSpeed * speedMultiplier);


        SetAnimation();
    }


    Vector3 nMeshPos = Vector3.zero;

    // rope animation transition
    IEnumerator Transition ( float t ) {

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


    void SetAnimation () {

        if (lastState == nextState) return;


        Debug.Log("Transitioning from " + lastState.ToString() + " to " + nextState.ToString());
        anim.SetTrigger(nextState.ToString());
        lastState = nextState;
    }
}
