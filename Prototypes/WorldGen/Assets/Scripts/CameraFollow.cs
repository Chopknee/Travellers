using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * A script that smooth transitions the camera to follow the player.
 * 
 */
public class CameraFollow : MonoBehaviour {

    public GameObject followTarget;
    [Tooltip("Controls how the camera settles on the target object when it stops moving.")]
    public AnimationCurve followCurve;
    [Range(0.5f, 4), Tooltip("Controls how far from the center of the camera the target can get.")]
    public float multiplier = 1;
    // Use this for initialization
    void Start () {
        if (followTarget == null) {
            followTarget = GameObject.FindGameObjectWithTag("Player");
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (followTarget != null) {
            
            //get the difference between the camera and player
            Vector3 diff = followTarget.transform.position - transform.position;

            //The z should be forced to 0 because we are on a 2d plane
            diff.y = 0;
            //Take the magnitude (distance from target) and scale it down. We only want the camera to move a portion of the distance from the target.
            //This number grows larger as the distance gets bigger, until the mul is equal to the distance moved by the target since the last frame.
            //Time.deltaTime ensures that this is the same on any framerate
            float mul = followCurve.Evaluate(diff.magnitude) * multiplier * Time.deltaTime;
            
            //This is now applied to the position
            diff = transform.position + (diff * mul);
            //Make sure it is within the current bounds.
            transform.position = diff;//Finally we can set the position of the camera!
        }
	}
}
