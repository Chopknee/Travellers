using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAnimDecisionMaker : MonoBehaviour
{
    private float idleDecision, attkDecision;
    public float currentClipLocation = 0, clipLength = 0;
    public int state = 1; // 1 = idle, 2 = moving, 3 = attacking (and moving?)
    private float last;

    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        last = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        var t = anim.GetCurrentAnimatorStateInfo(0);
        currentClipLocation = t.normalizedTime;
        clipLength = t.length;
        if (t.normalizedTime >= 1.5f && Time.time - last >= 1)
        {
            float r = (float)(Mathf.FloorToInt(Random.Range(0,5)));
            Debug.Log("Random Decision for animations: " + r);
            last = Time.time;
            switch (state)
            {
                case 1:
                    anim.SetFloat("RandIdle", r);
                    break;
                case 2:
                    anim.SetFloat("RandAttk", r);
                    break;
                case 3:
                    break;
            }
        }
    }
}
