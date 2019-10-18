using UnityEngine;

public class CameraMovement: MonoBehaviour {
    public Transform currentTarget;
    [Range(0.1f, 50f)]
    public float smoothness = 15f;
    [Range(0.1f, 50f)]
    public float pan = 15f;
    [Range(0.1f, 5f)]
    public float panSpeed = 2f;
    [Range(.01f, 1f)]
    public float zoomSpeed = 1f;
    public float mouseScrollSensitivity = 1f;
    public float mousePanSensitivity = 1.5f;
    public float axisScrollSensitivity = 1f;
    public float axisPanSensititivty = 1f;
    public bool CONTROLLER_INPUT = true;
    public float camMinHeight = 1, camMaxHeight = 15;
    float camHeightRange;
    public float camMinDist = 0.64f, camMaxDist = 1.1f;
    float distRange;
    public float height = 5f;

    Vector3 camOffset;

    public float distToPlayer;
    private void Start () {
        camHeightRange = camMaxHeight - camMinHeight;
        distRange = camMaxDist - camMinDist;
        height = camMaxHeight / 2f;
        camOffset = new Vector3(height, height, height);
    }

    private void LateUpdate () {

        camHeightRange = camMaxHeight - camMinHeight;
        distRange = camMaxDist - camMinDist;

        float horizontalAxis = 0;
        float verticalAxis = 0;

        if (CONTROLLER_INPUT) {
            horizontalAxis = Input.GetAxisRaw("Horizontal") * axisScrollSensitivity;
            verticalAxis = Input.GetAxisRaw("Vertical") * axisPanSensititivty;
        } else {
            if (Input.GetMouseButton(2)) {
                horizontalAxis = Input.GetAxis("Mouse X") * mousePanSensitivity;
            }
            verticalAxis = Input.mouseScrollDelta.y * mouseScrollSensitivity;
        }
        //This is how big the movement for the camera should be.

        if (horizontalAxis != 0) {
            //rotation offset of camera (camera physically moves around the player)
            camOffset = Quaternion.AngleAxis(horizontalAxis * panSpeed, Vector3.up) * camOffset;
        }

        Vector3 targetPos = currentTarget.position;

        //Zooming
        height -= verticalAxis * zoomSpeed;
        height = Mathf.Clamp(height, camMinHeight, camMaxHeight);

        distToPlayer = ((height / camHeightRange) * distRange) + camMinDist;

        //offset is equal to target pos, plus the vector containing the offset of camoffset x and y
        Vector3 posWithOffset = ( ( targetPos + ( new Vector3(camOffset.x * distToPlayer, height, camOffset.z * distToPlayer))));

        // position of camera
        Vector3 lerpingPos = Vector3.Lerp(transform.position, posWithOffset, smoothness * Time.deltaTime);
        transform.position = lerpingPos;

        //rotation of camera
        var heading = targetPos - transform.position;
        var rot = Quaternion.LookRotation(heading);

        //lerp of actual camera rotation (custom LookAt)
        Quaternion rotLerp = Quaternion.Lerp(transform.rotation, rot, pan * Time.deltaTime);
        transform.rotation = rotLerp;
    }
}