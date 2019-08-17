using Photon.Pun;
using UnityEngine;
/**
 * A script that smooth transitions the camera to follow the player.
 * 
 */
namespace BaD.Chopknee.Utilities {
    public class CameraFollow: MonoBehaviour {

        public Transform currentTarget;
        public float smoothness = 5f, pan = 5f;
        public Vector3 camOffset, offsetY;
        public float turnSpeed = 2f, offset = 5f;
        public float verticalOffset = 5f;
        public Vector2 verticalLimits = new Vector2(0, 10);
        public Vector2 offsetLimits = new Vector2 (0, 10);
        [Range(.01f, 1f)]
        public float zoomSensitivity = 1f;
        public float distanceToPlayer = 2f;
        public float horizontalDistanceToPlayer = 3f;
        Vector3 targetPos = Vector3.zero;

        Vector3 playerPrevPos, playerMoveDir;

        private void Start () {
            camOffset = new Vector3(offset, verticalOffset, offset);
            playerPrevPos = targetPos;

        }

        private void Update () {
            //if (currentTarget == null) {
            //    //Attempt to assign the camera to the current player.
            //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            //    foreach (GameObject go in players) {
            //        PhotonView pv = go.GetComponent<PhotonView>();
            //        if (pv == null || pv.Owner == null || !pv.Owner.IsLocal) {
            //            continue;
            //        }
            //        currentTarget = go.transform;
            //    }
            //}
        }
        private void LateUpdate () {
            if (currentTarget == null) return;
            targetPos = currentTarget.position;

            //offset is equal to target pos, plus the vector containing the offset of camoffset x and y, multiplied by the offset float (5f);
            Vector3 posWithOffset = ( ( targetPos + ( new Vector3(camOffset.x, 0 + verticalOffset, camOffset.z) ) * offset ) );



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
        void FixedUpdate () {
            if (currentTarget == null) return;
            //zoom on scroll
            if (Input.mouseScrollDelta != Vector2.zero) {
                offset -= Input.mouseScrollDelta.y * zoomSensitivity;
                verticalOffset -= Input.mouseScrollDelta.y * zoomSensitivity; // this should be based on ground 0 instead of player's location.

                offset = Mathf.Max(Mathf.Min(offset, offsetLimits.y), offsetLimits.x);
                verticalOffset = Mathf.Max(Mathf.Min(verticalOffset, verticalLimits.y), verticalLimits.x);
            }


        }

        //public GameObject followTarget;
        //[Tooltip("Controls how the camera settles on the target object when it stops moving.")]
        //public AnimationCurve followCurve;
        //[Range(0.5f, 4), Tooltip("Controls how far from the center of the camera the target can get.")]
        //public float multiplier = 1;
        //// Use this for initialization
        //void Start () {
        //    if (followTarget == null) {
        //        followTarget = GameObject.FindGameObjectWithTag("Player");
        //    }
        //}

        // Update is called once per frame
        //void Update () {
        //    if (followTarget != null) {

        //        //get the difference between the camera and player
        //        Vector3 diff = followTarget.transform.position - transform.position;

        //        //The z should be forced to 0 because we are on a 2d plane
        //        diff.y = 0;
        //        //Take the magnitude (distance from target) and scale it down. We only want the camera to move a portion of the distance from the target.
        //        //This number grows larger as the distance gets bigger, until the mul is equal to the distance moved by the target since the last frame.
        //        //Time.deltaTime ensures that this is the same on any framerate
        //        float mul = followCurve.Evaluate(diff.magnitude) * multiplier * Time.deltaTime;

        //        //This is now applied to the position
        //        diff = transform.position + ( diff * mul );
        //        //Make sure it is within the current bounds.
        //        transform.position = diff;//Finally we can set the position of the camera!
        //    } else {
        //        //Attempt to assign the camera to the current player. (THIS IS ONLY A TEMPORAORY CODE UNTIL THIS WORKOLI)
        //        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //        foreach (GameObject go in players) {
        //            PhotonView pv = go.GetComponent<PhotonView>();
        //            if (pv == null || pv.Owner == null || !pv.Owner.IsLocal) {
        //                continue;
        //            }
        //            followTarget = go;
        //        }
        //    }
        //}
    }
}
