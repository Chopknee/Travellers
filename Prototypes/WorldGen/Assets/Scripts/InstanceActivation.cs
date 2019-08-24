using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Terrain;
using UnityEngine;

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
        //Allows the player to 'cancel' the move to this point.
        if (isCurrentNavTarget && Input.GetButtonDown("Interact")) {
            if (!isHighlighted) {
                isCurrentNavTarget = false;
            }
        }

        if (isCurrentNavTarget) {
            GameObject playerInst;
            if (DungeonManager.CurrentInstance == null) {
                playerInst = MainControl.Instance.LocalPlayerObjectInstance;
            } else {
                playerInst = DungeonManager.CurrentInstance.playerInstance;
            }
            if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
                //Interact with the thing.
                DoInteraction();
            }
        }
    }

    public void OnMouseDown () {
        GameObject playerInst;
        if (DungeonManager.CurrentInstance == null) {
            playerInst = MainControl.Instance.LocalPlayerObjectInstance;
        } else {
            playerInst = DungeonManager.CurrentInstance.playerInstance;
        }
        if (( playerInst.transform.position - transform.position ).sqrMagnitude < activationRadiusSquared) {
            DoInteraction();
        } else {
            //Set this as the target and somehow make a callback to this when the player is close enough?
            // - unless the player clicks elsewhere.
            isCurrentNavTarget = true;
            if (DungeonManager.CurrentInstance == null) {
                MainControl.Instance.LocalPlayerObjectInstance.GetComponent<Player>().SetDestination(transform.position);
            } else {
                DungeonManager.CurrentInstance.playerInstance.GetComponent<PlayerMovement>().SetDestination(transform.position);
            }
            
        }
    }

    public void DoInteraction() {
        Debug.Log("Entering village instance!");
        //Generating a seed based on the position and current seed stack. (meaning there is a limit to how many levels deep we can go)
        int instanceSeed = MainControl.Instance.GetStackSeed() + Choptilities.Vector3ToID(transform.position);
        //int instanceSeed = //Seed based on position?
        if (dungeonManagerPrefab != null) {
            GameObject dungeonManagerInst = Instantiate(dungeonManagerPrefab);
            dungeonManagerInst.transform.position = Vector3.zero;
            dungeonManagerInst.transform.localScale = Vector3.one;
            dungeonManager = dungeonManagerInst.GetComponent<DungeonManager>();
            dungeonManager.name = "Instance Manager " + instanceSeed;
            dungeonManager.GeneratorSeed = instanceSeed;
            dungeonManager.EnterArea();
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