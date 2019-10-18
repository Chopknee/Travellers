using BaD.Modules.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BaD.Modules.Control {

    public class PlayerMovement: MonoBehaviour {

        public LayerMask layer_mask;
        public Vector3 destinationPosition;
        Quaternion targetRotation;
        public GameObject arrow;
        public WaypointAnimations currentArrow;
        NavMeshAgent agent;
        public GameObject particles;
        public float playerSpeed = 1;

        MainControls mc;
        public Vector2 moveDirection;

        GameObject cameraTracker;
        Transform cameraTrackTransform;
        Transform cameraTransform;

        private void Start () {
            destinationPosition = transform.position;
            agent = GetComponent<NavMeshAgent>();

            mc = MainControl.Instance.Controls;

            cameraTracker = new GameObject("Camera Tracker");
            cameraTracker.transform.SetParent(transform);
            cameraTrackTransform = cameraTracker.transform;
            cameraTransform = Camera.main.gameObject.transform;
        }

        private void Update () {
            //Make the camera track point in the forward of the camera, minus the pitch
            cameraTrackTransform.rotation = Quaternion.identity;
            cameraTrackTransform.position = new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
            cameraTrackTransform.forward = ( transform.position - cameraTransform.position );

            

            moveDirection = mc.Player.Movement.ReadValue<Vector2>();
            if (moveDirection != Vector2.zero) {
                Vector3 dir = (moveDirection.y * cameraTrackTransform.forward.normalized) + (moveDirection.x * cameraTrackTransform.right.normalized);
                dir.Normalize();
                dir *= playerSpeed * Time.deltaTime;
                agent.Move(dir);
                destinationPosition = transform.position + dir;
                //MoveToPoint(destinationPosition);
                transform.rotation = FaceDirection(destinationPosition);
            }

            
        }


        public void MoveToPoint ( Vector3 dest ) {
            Vector3 dist = dest - transform.position;

            if (dist.sqrMagnitude > agent.stoppingDistance * agent.stoppingDistance) {
                Vector3 movement = (transform.position + dist) * Time.deltaTime;
                movement.y = transform.position.y;
                agent.Move(movement);
            }


        }

        public Quaternion FaceDirection ( Vector3 destination ) {
            float pan = 10f;
            //rotation of Object
            var heading = destination - transform.position;
            var rot = Quaternion.LookRotation(heading);

            Quaternion rotLerp = Quaternion.Lerp(transform.rotation, rot, pan * Time.deltaTime);

            return rotLerp;
        }
    }
}