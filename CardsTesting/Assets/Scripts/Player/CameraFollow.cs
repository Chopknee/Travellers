using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform player;


    public float smoothness = 5f, pan = 5f;
    [Range(-100f, 100f)]
    public float offset = 5f;
    public bool followPlayer = true;

    public void FixedUpdate()
    {
        Vector3 posWithOffset = (new Vector3(player.position.x, transform.position.y, player.position.z + offset));

        // position of camera
        Vector3 lerpingPos = Vector3.Lerp(transform.position, posWithOffset, smoothness * Time.deltaTime * .1f);
        transform.position = lerpingPos;

        
        

        var heading = player.position - transform.position;
        
        var rot = Quaternion.LookRotation(heading);
        Quaternion rotLerp = Quaternion.Lerp(transform.rotation, rot, pan * Time.deltaTime * .1f);
        transform.rotation = rotLerp;

        //transform.LookAt(player);


    }

}
