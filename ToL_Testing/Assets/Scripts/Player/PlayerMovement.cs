

using BaD.Modules.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement : MonoBehaviour
{
    public LayerMask layer_mask;
    public Vector3 destinationPosition;
    Quaternion targetRotation;
    public GameObject arrow;
    public WaypointAnimations currentArrow;
    NavMeshAgent agent;
    //public bool followingArrow;
    public Transform clickedObject;
    public GameObject particles;
    
    
    public GameObject CONTROLLER_VISUAL_DIRECTION_OBJECT;
    public bool CONTROLLER_INPUT = true;
    public bool playerCanInteract = true;
    public bool faceDirection;
    public Vector2 CONTROLLER_RightStickDirection, CONTROLLER_LeftStickDirection;

    public float playerSpeedBase = 1, playerspeedModifier = 1;

    private void Start()
    {
        destinationPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();

        

        if (CONTROLLER_VISUAL_DIRECTION_OBJECT)
        {
            CONTROLLER_VISUAL_DIRECTION_OBJECT.transform.position = transform.position;
            Camera.main.GetComponent<CameraMovement>().ControllerVisualObject = CONTROLLER_VISUAL_DIRECTION_OBJECT.transform;
            
        }
    }

    private void Update()
    {
        if (!playerCanInteract)
        {
            return;
        }

        if (destinationPosition != null)
        {
            transform.rotation = FaceDirection(destinationPosition);

            if (!CONTROLLER_INPUT && (transform.position - destinationPosition).sqrMagnitude > 49)
            {
                SetDestination(destinationPosition, false);
            }

        }
        // Controller input axis
        if (CONTROLLER_INPUT)
        {
            //moveDirection = mc.Player.Movement.ReadValue<Vector2>();

            CONTROLLER_VISUAL_DIRECTION_OBJECT.transform.position = transform.position;

            float verticalRot;
            //CONTROLLER_RightStickDirection = new Vector2(Input.GetAxisRaw("HorizontalRightStick"), Input.GetAxisRaw("VerticalRightStick"));
            CONTROLLER_RightStickDirection = new Vector2(ControlManager.mainControls.Player.CameraHorizontal.ReadValue<float>(), ControlManager.mainControls.Player.CameraVertical.ReadValue<float>());
            CONTROLLER_LeftStickDirection = ControlManager.mainControls.Player.Movement.ReadValue<Vector2>();
            //CONTROLLER_LeftStickDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            verticalRot = Mathf.Atan2(CONTROLLER_LeftStickDirection.x, CONTROLLER_LeftStickDirection.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            //Quaternion.AngleAxis(horizontalAxis * mouseSensitivity * turnSpeed, Vector3.up) * camOffset;
            CONTROLLER_VISUAL_DIRECTION_OBJECT.transform.eulerAngles = new Vector3(0, verticalRot, 0);


            if (CONTROLLER_LeftStickDirection != Vector2.zero)
            {
                destinationPosition = CONTROLLER_VISUAL_DIRECTION_OBJECT.transform.Find("Target").position;
                SetDestination(destinationPosition, false);
            }
        }
        else if (Input.GetButtonDown("Interact"))

        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            // PC Input
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Camera.main.farClipPlane, layer_mask))
            {
                string t = hit.collider.tag;
                clickedObject = hit.transform;
                Vector3 directionOfTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                targetRotation = Quaternion.LookRotation(directionOfTarget - transform.position);
                if (t == ("Room"))
                {
                    Camera.main.GetComponent<CameraMovement>().playerIsMoving = true;
                    destinationPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    SetDestination(destinationPosition, true);
                }
            }
        }



        #region oldArrowStuff
        //if (currentArrow != null)
        //{
        //    if (!agent.pathPending)
        //    {
        //        if (agent.remainingDistance <= agent.stoppingDistance)
        //        {
        //            if (!agent.hasPath || GetComponent<MovementAnimationController>().speed <= 0)
        //            {
        //                KillArrow();
        //                particles.GetComponent<ParticleSystem>().Play();
        //            }
        //        }
        //    }

        //    if (!agent.pathPending && !agent.hasPath)
        //    {
        //        timeSincePathfinding += .1f;
        //        Camera.main.GetComponent<CameraMovement>().playerIsMoving = true;
        //    }
        //    else
        //    {
        //        timeSincePathfinding = 0;
        //        Camera.main.GetComponent<CameraMovement>().playerIsMoving = false;
        //    }

        //    if (timeSincePathfinding >= 5f)
        //    {
        //        KillArrow();
        //    }
        //}


        //if (currentArrow != null && followingArrow)
        //{
        //    if (Vector3.Distance(transform.position, currentArrow.transform.position) < .1f)
        //    {
        //        KillArrow();
        //    }
        //}
        #endregion

    }

    float timeSincePathfinding = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PortalOut"))
        {
            Debug.LogError("----------------------Exited----------------------");
        }
    }

    //private void OnDestroy()
    //{
    //    if (currentArrow != null)
    //    {
    //        currentArrow.Die();
    //        currentArrow = null;
    //    }
    //}
    //private void OnDisable()
    //{
    //    if (currentArrow != null)
    //    {
    //        currentArrow.Die();
    //        currentArrow = null;
    //    }
    //}

    public Quaternion FaceDirection(Vector3 destination)
    {
        float pan = 10f;
        //rotation of Object
        var heading = destination - transform.position;
        var rot = Quaternion.LookRotation(new Vector3(heading.x, transform.position.y, heading.z));

        Quaternion rotLerp = Quaternion.Lerp(transform.rotation, rot, pan * Time.deltaTime);

        return rotLerp;
    }
    public void MoveToPoint(Vector3 dest)
    {
        float dist = Vector3.Distance(dest, transform.position);

        if (dist > agent.stoppingDistance)
        {
            Vector3 movement = transform.forward * Time.deltaTime * dist * playerSpeedBase;// * CONTROLLER_LeftStickDirection;
            movement.y = transform.position.y;
            agent.Move(movement);
        }


    }

    public void SetDestination(Vector3 dest, bool hasArrow) // location to move to, does it have an arrow with it
    {
        if (!playerCanInteract) return;

        if (destinationPosition != null && targetRotation != null && destinationPosition != transform.position)
        {

            //if (currentArrow != null)
            //{
            //    KillArrow();
            //}

            MoveToPoint(destinationPosition);


            //GetComponent<NavMeshAgent>().SetDestination(destinationPosition);

            //if (hasArrow)
            //    CreateArrow(dest);
        }
    }


    //public void CreateArrow(Vector3 dest)
    //{
    //    if (!playerCanInteract) return;

    //    GameObject o = null;
    //    o = Instantiate(arrow, new Vector3(dest.x, 0, dest.z), Quaternion.Euler(new Vector3(-90, 0, 0)));

    //    if (Physics.Raycast(dest, Vector3.down, out RaycastHit hit, 500, layer_mask))
    //    {
    //        o.transform.position = hit.point + Vector3.up * 3.14f;
    //    }

    //    currentArrow = o.transform.Find("wp").GetComponent<WaypointAnimations>();

    //    followingArrow = true;

    //}

    //public void KillArrow()
    //{
    //    if (currentArrow != null)
    //    {
    //        currentArrow.Die();
    //        currentArrow = null;
    //    }
    //}
}
