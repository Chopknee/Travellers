using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Control;
using BaD.Modules.Terrain;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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
    
    private float activationRadiusSquared;

    GameObject pl {
        get {
            GameObject playerInst;
            if (DungeonManager.CurrentInstance == null) {
                playerInst = MainControl.Instance.LocalPlayerObjectInstance;
            } else {
                playerInst = DungeonManager.CurrentInstance.LocalDungeonPlayerInstance;
            }
            return playerInst;
        }
    }

    public void Start () {
        if (HoverInfoWindowPrefab != null) {
            HoverInfoWindowGameobject = Instantiate(HoverInfoWindowPrefab);
            HoverInfoWindowGameobject.GetComponent<UITargetObject>().target = transform;
            HoverInfoWindowGameobject.SetActive(false);
            //HoverInfoWindowGameobject.transform.SetParent(MainControl.Instance.ActionConfirmationUI.transform);
        }
        activationRadiusSquared = activationRadius * activationRadius;

        //MainControl.Instance.Controls.Player.Interact.performed += context => OnInteract();


    }

    public void OnInteract(InputAction.CallbackContext contex) {
        if (enabled) {
            if ((pl.transform.position - transform.position).sqrMagnitude < activationRadiusSquared) {
                DoInteraction();//Do the interaction
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

    private void OnTriggerEnter ( Collider other ) {
        if (other.gameObject.CompareTag("Player")) {
            if (HoverInfoWindowGameobject != null) {
                HoverInfoWindowGameobject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit ( Collider other ) {
        if (other.gameObject.CompareTag("Player")) {
            if (HoverInfoWindowGameobject != null) {
                HoverInfoWindowGameobject.SetActive(false);
            }
        }
    }

    public void OnEnable() {
        MainControl.Instance.Controls.Player.Interact.performed += OnInteract;
    }

    public void OnDisable() {
        MainControl.Instance.Controls.Player.Interact.performed -= OnInteract;
        if (HoverInfoWindowGameobject != null) {
            HoverInfoWindowGameobject.SetActive(false);
        }
    }

    public void OnDrawGizmosSelected () {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}