using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform currentTarget;

    public float smoothness = 5f, pan = 5f;

    public Vector3 camOffset, offsetY;
    public float turnSpeed = 2f, offset = 5f;
    public float offsetClamped = 5f;
    public float verticalOffset = 5f;
    public float verticalOffsetClamped = 5f;
    [Range(.01f, 1f)]
    public float zoomSensitivity = 1f;
    public bool followPlayer = true;
    public float distanceToPlayer = 5f;
    Vector3 targetPos = Vector3.zero;
    
    Vector3 playerPrevPos, playerMoveDir;

    private void Start()
    {
        camOffset = new Vector3(distanceToPlayer, verticalOffset, distanceToPlayer);
        playerPrevPos = targetPos;
        
    }
    private void LateUpdate()
    {
        targetPos = currentTarget.position;

        Vector3 posWithOffset = ((targetPos + (new Vector3(camOffset.x, 0 + verticalOffset, camOffset.z)) * offset));
        // position of camera
        Vector3 lerpingPos = Vector3.Lerp(transform.position, posWithOffset, smoothness * Time.deltaTime);
        transform.position = lerpingPos;



        //rotation offset of camera (camera physically moves around the player)
        camOffset = Quaternion.AngleAxis(-Input.GetAxis("Horizontal") * turnSpeed, Vector3.up) * camOffset;

        //rotation of camera
        var heading = targetPos - transform.position;
        var rot = Quaternion.LookRotation(heading);

        //lerp of actual camera rotation (custom LookAt)
        Quaternion rotLerp = Quaternion.Lerp(transform.rotation, rot, pan * Time.deltaTime);
        transform.rotation = rotLerp;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //zoom on scroll
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            distanceToPlayer -= Input.mouseScrollDelta.y * zoomSensitivity;
            offset -= Input.mouseScrollDelta.y * zoomSensitivity;
        }

        verticalOffset += Input.GetAxis("Vertical") * zoomSensitivity; // this should be based on ground 0 instead of player's location.
        
    }

    



}
