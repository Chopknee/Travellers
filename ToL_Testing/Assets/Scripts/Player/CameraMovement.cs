using BaD.Modules.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform currentTarget;

    public float smoothness = 5f, pan = 5f;

    public Vector3 camOffset, offsetY;
    public float turnSpeed = 2f, offset = 5f;
    public float verticalOffset = 5f;
    [Range(.01f, 1f)]
    public float zoomSensitivity = 1f;
    public bool followPlayer = true;
    public float distanceToPlayer = 2f;
    public float horizontalDistanceToPlayer = 3f;
    public float mouseSensitivity = 1.5f;
    public float camMinDist = 1, camMaxDist = 7;
    Vector3 targetPos = Vector3.zero;
    float velocity = 0, maxVelocity = 5f;

    Vector3 playerPrevPos, playerMoveDir;

    public bool CONTROLLER_INPUT = true;
    float horizontalAxis, verticalAxis;

    public Transform ControllerVisualObject;

    public bool playerIsMoving;
    MainControls mainControls;

    private void Start()
    {
        mainControls = ControlManager.mainControls;
        camOffset = new Vector3(offset, verticalOffset, offset);
        playerPrevPos = targetPos;

    }
    private void LateUpdate()
    {

        if (CONTROLLER_INPUT)
        {
            horizontalAxis = mainControls.Player.CameraHorizontal.ReadValue<float>();
            verticalAxis = mainControls.Player.CameraVertical.ReadValue<float>();
        }

        if (playerIsMoving)
            targetPos = currentTarget.position + currentTarget.forward * 200; //Vector3.Cross(currentTarget.position, ControllerVisualObject.Find("Target").position);
        else
            targetPos = currentTarget.position;


        //offset is equal to target pos, plus the vector containing the offset of camoffset x and y, multiplied by the offset float (5f);
        Vector3 posWithOffset = ((targetPos + (new Vector3(camOffset.x, 0 + verticalOffset, camOffset.z)) * offset));
        

        // position of camera
        Vector3 lerpingPos = Vector3.Lerp(transform.position, posWithOffset, smoothness * Time.deltaTime);
        transform.position = lerpingPos;
        

        if (horizontalAxis != 0)
        {
            //rotation offset of camera (camera physically moves around the player)
            camOffset = Quaternion.AngleAxis(horizontalAxis * turnSpeed, Vector3.up) * camOffset;
        }


        if (velocity < maxVelocity) velocity += turnSpeed * horizontalAxis;


        if (Input.GetMouseButton(2))
        {
            camOffset = Quaternion.AngleAxis(horizontalAxis * mouseSensitivity * turnSpeed, Vector3.up) * camOffset;
        }
        else
        {
            if (velocity > 0) velocity -= .01f;
            else velocity = 0;
        }




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
            offset -= Input.mouseScrollDelta.y * zoomSensitivity;
            offset = Mathf.Clamp(offset, camMinDist, camMaxDist);
            verticalOffset -= Input.mouseScrollDelta.y * zoomSensitivity; // this should be based on ground 0 instead of player's location.
            verticalOffset = Mathf.Clamp(verticalOffset, camMinDist, camMaxDist);
        }
        else
        {
            offset -= verticalAxis * zoomSensitivity;
            offset = Mathf.Clamp(offset, camMinDist, camMaxDist);
            verticalOffset -= verticalAxis * zoomSensitivity; // this should be based on ground 0 instead of player's location.
            verticalOffset = Mathf.Clamp(verticalOffset, camMinDist, camMaxDist);
        }



    }





}
