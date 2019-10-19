using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementAnimationController: MonoBehaviour {

    public enum state {
        Attack, Die
    }

    public state lastState, nextState;

    public float speed;
    private Vector3 velocity;

    public Animator anim;

    AnimationClip lastClip;
    NavMeshAgent agent;
    Vector3 lastPos, nextPos;

    public Average avg;

    private void Start () {
        lastPos = transform.position;

        if (anim == null) {
            anim = transform.GetComponent<Animator>();
            if (anim == null) {
                Debug.LogFormat("<color=blue>Missing</color> animation controller on gameobject {0} disabling movement animation controller.", gameObject.name);
                this.enabled = false;
            }
        }
        agent = GetComponent<NavMeshAgent>();
        avg = new Average(2);
    }

    float avgSpeed;
    float smoothVel = 0;
    float lastSpd = 0;

    private void LateUpdate () {
        //This is all for the running animation speed setting
        nextPos = transform.position;
        float lv = ( nextPos - lastPos ).sqrMagnitude / Time.deltaTime;
        float tmpSpeed = 100 * Mathf.SmoothDamp(lv, (float) System.Math.Round(lv, 1), ref smoothVel, .5f);
        speed = Mathf.Lerp(lastSpd, avgSpeed, .3f);
        lastSpd = tmpSpeed;
        speed = (float) System.Math.Round(speed, 2);
        lastPos = transform.position;

        avgSpeed = (float) System.Math.Round(avg.GetNext(speed), 2);

        anim.SetFloat("Runspeed", avgSpeed);

        SetAnimation();
    }


    Vector3 nMeshPos = Vector3.zero;

    void SetAnimation () {
        if (lastState == nextState) return;

        Debug.Log("Transitioning from " + lastState.ToString() + " to " + nextState.ToString());
        anim.SetTrigger(nextState.ToString());
        lastState = nextState;
    }
}
