using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Control;
using BaD.Modules.Terrain;
using UnityEngine;
using UnityEngine.AI;

public class InstanceActivation : MonoBehaviour {
    [SerializeField]
#pragma warning disable 0649
    private GameObject dungeonManagerPrefab;

    private DungeonManager dungeonManager;
    [SerializeField]
#pragma warning disable 0649
    private GameObject HoverInfoWindowPrefab;
    private GameObject HoverInfoWindowGameobject;

    public float activationRadius = 15;

    public bool isCurrentNavTarget = false;
    public bool isHighlighted = false;
    private float activationRadiusSquared;
    public void Start () {
        if (HoverInfoWindowPrefab != null) {
            HoverInfoWindowGameobject = Instantiate(HoverInfoWindowPrefab);
            HoverInfoWindowGameobject.GetComponent<UITargetObject>().target = transform;
            HoverInfoWindowGameobject.SetActive(false);
            HoverInfoWindowGameobject.transform.SetParent(MainControl.Instance.ActionConfirmationUI.transform);
        }
        activationRadiusSquared = activationRadius * activationRadius;
    }

    public void Update () {
        //If interact button is clicked
        if (Input.GetButtonDown("Interact")) {
            //If the mouse is not over the collider
            if (!isHighlighted) {
                //If the player is already navigating to this spot.
                if (isCurrentNavTarget) {
                    //Cancel the move
                    isCurrentNavTarget = false;
                }

                //If the mouse is over the collider
            } else {
                //If the player is not already moving here
                if (!isCurrentNavTarget) {
                    GameObject playerInst;
                    if (DungeonManager.CurrentInstance == null) {
                        playerInst = MainControl.Instance.LocalPlayerObjectInstance;
                    } else {
                        playerInst = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance;
                    }
                    //If the player is close enough to this object
                    if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
                        DoInteraction();//Do the interaction
                    } else {
                        //Try to find a spot close to this thing.
                        NavMesh.SamplePosition(transform.position, out NavMeshHit hit, activationRadius + 16, NavMesh.AllAreas);
                        if (!hit.hit) { Debug.Log("The instance activator " + gameObject.name + " is unreachable, apparently."); return; }

                        isCurrentNavTarget = true;
                        if (DungeonManager.CurrentInstance == null) {
                            //MainControl.Instance.LocalPlayerObjectInstance.GetComponent<PlayerMovement>().SetDestination(hit.position, true);
                        } else {
                            //DungeonManager.CurrentInstance.LocalDungeonPlayerInstance.GetComponent<PlayerMovement>().SetDestination(hit.position, true);
                        }
                    }
                }
            }
        }
        if (isCurrentNavTarget) {
            GameObject playerInst;
            if (DungeonManager.CurrentInstance == null) {
                playerInst = MainControl.Instance.LocalPlayerObjectInstance;
            } else {
                playerInst = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance;
            }
            if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
                //Interact with the thing.
                isCurrentNavTarget = false;
                DoInteraction();
            }
        }
    }

    public void DoInteraction() {
        Transform t = transform.Find("ExitPoint");
        if (t != null) {
            MainControl.Instance.playerDropoffSpot = t;
        }

        //Generating a seed based on the position and current seed stack. (meaning there is a limit to how many levels deep we can go)
        int instanceSeed = MainControl.Instance.GetStackSeed() + Choptilities.Vector3ToID(transform.position);
        //If the dungeon manager is previously cached
        GameObject dm = GameObject.Find("Instance Manager " + instanceSeed);
        if (dm != null) {
            dm.GetComponent<DungeonManager>().EnterArea(instanceSeed);
            return;
        }
        //int instanceSeed = //Seed based on position?
        if (dungeonManagerPrefab != null) {
            Debug.Log("Entering new area; " + instanceSeed);
            GameObject dungeonManagerInst = Instantiate(dungeonManagerPrefab);
            dungeonManagerInst.name = "Instance Manager " + instanceSeed;
            dungeonManager = dungeonManagerInst.GetComponent<DungeonManager>();
            dungeonManager.EnterArea(instanceSeed);
        } else {
            //If no dungeon manager is set, assume the player wishes to exit.
            Debug.Log("Exiting area.");
            DungeonManager.CurrentInstance.ExitArea();
        }
    }

    public void OnMouseEnter () {
        if (HoverInfoWindowGameobject != null) {
            HoverInfoWindowGameobject.SetActive(true);
            isHighlighted = true;
        }
    }

    public void OnMouseExit () {
        if (HoverInfoWindowGameobject != null) {
            HoverInfoWindowGameobject.SetActive(false);
            isHighlighted = false;
        }
    }

    public void OnDisable () {
        if (HoverInfoWindowGameobject != null) {
            HoverInfoWindowGameobject.SetActive(false);
            isHighlighted = false;
        }
    }

    public void OnDrawGizmos () {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}