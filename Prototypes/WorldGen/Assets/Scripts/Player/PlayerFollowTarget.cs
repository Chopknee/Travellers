using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowTarget : MonoBehaviour
{

    public Transform currentTarget;

    public float smoothness = 5f, pan = 5f;
    
    [Range(.01f, 1f)]
    public float zoomSensitivity = 1f;
    public bool followPlayer = true;

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 posWithOffset = ((currentTarget.position));

        followPlayer = (Vector3.Distance(transform.position, currentTarget.position) > .2f);


        if (followPlayer)
        {
            // position of camera
            //Vector3 lerpingPos = Vector3.Lerp(transform.position, posWithOffset, smoothness * Time.deltaTime * .1f);
            //transform.position = lerpingPos;

            Vector3 dir = currentTarget.position - transform.position;
            transform.position += dir * Time.deltaTime * smoothness;




            var heading = currentTarget.position - transform.position;

            var rot = Quaternion.LookRotation(heading);
            Quaternion rotLerp = Quaternion.Lerp(transform.rotation, rot, pan * Time.deltaTime * .1f);
            transform.rotation = rotLerp;
        }
        //transform.LookAt(player);

    }
}
