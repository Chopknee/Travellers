using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Combat;
using BaD.Modules.Control;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetInstanceManager))]
public class DungeonManager: MonoBehaviour {

    public static DungeonManager CurrentInstance { get; private set; }
    
    public bool Showing { get; private set; }

    public GameObject LocalDungeonPlayerInstance { get; private set; }

    public GameObject dungeonInstance { get; private set; }

    [SerializeField]
#pragma warning disable 0649
    private GameObject dungeonPrefab;
    [SerializeField]
#pragma warning disable 0649
    private Vector3 navmeshAreaSize;
    [SerializeField]
#pragma warning disable 0649
    private Vector3 navmeshAreaCenter;
    [SerializeField]
#pragma warning disable 0649
    private LayerMask navMeshLayers;

    public int dungeonSeed { get; private set; }


    public AnimationCurve fadeCurve;
    public Color fadeColor;
    public float fadeTime;
    public float fadeWaitTime;
    UIFade uif;

    private bool instantiated = false;
    private NavMeshSurface navSurface;
    private Transform lastLocalPlayerLocation;
    private Vector3 lastPlayerLocation;
    /**
     * Entering an instance is as follows;
     * Hide the map. -- will need to set up for instance to instance transfer
     * Make the dungeon manager object, if it does not already exist.
     * Make the nav mesh surface with settings, if it does not already exist
     * Generate the instance
     * Generate the navmesh
     * Spawn the player object and position it in the world
     * Set up camera for instances
     */

    //Automatically hides everything and generates the dungeon instance
    public void EnterArea(int seedId) {
        dungeonSeed = seedId;//USED FOR NET COMMS
        if (CurrentInstance != null) {
            SetLocalPlayerControl(false);
        } else {
            MainControl.SetLocalPlayerControl(false);//Freeze the overworld player
        }

        //First things first, gotta prepare the space for the transiton!!
        uif = UIFade.DoFade(fadeTime, OnEnterDungeonFadeFinished, fadeColor, fadeCurve);

    }

    public void ExitArea() {
        MainControl.Instance.ExitArea(this);
        //HideArea();
    }

    //Exits the area without running the world exit area code.
    public void HideArea() {
        //Hides all gameobjects and disables the instance
        lastPlayerLocation = LocalDungeonPlayerInstance.transform.position;

        dungeonInstance.SetActive(false);
        //PhotonNetwork.Destroy(playerInstance);
        NetInstanceManager netManager = GetComponent<NetInstanceManager>();
        netManager.DestroyObject(LocalDungeonPlayerInstance);
        //Shut down the instance manager.
        netManager.LeaveInstance();
        netManager.enabled = false;
        navSurface.enabled = false;

        if (CurrentInstance == this) {
            CurrentInstance = null;
        }

        Showing = false;

    }

    //Spawns the dungeon and sets up the networking for it
    void OnEnterDungeonFadeFinished() {
        MainControl.Instance.EnterArea(this);

        NetInstanceManager netManagerSettings = GetComponent<NetInstanceManager>();

        CurrentInstance = this;

        //Generate the nav mesh - add the nav mesh component if it is not already present.
        navSurface = GetComponent<NavMeshSurface>();
        if (navSurface == null) {
            navSurface = gameObject.AddComponent<NavMeshSurface>();
            navSurface.collectObjects = CollectObjects.Volume;
            navSurface.size = navmeshAreaSize;
            navSurface.center = navmeshAreaCenter;
            navSurface.overrideTileSize = true;
            navSurface.voxelSize = 0.126667f;
            navSurface.overrideTileSize = true;
            navSurface.tileSize = 32;
            navSurface.layerMask = navMeshLayers;
        } else {
            navSurface.enabled = true;
        }

        if (!instantiated) {//Spawn in the dungeon prefab and build the navmesh
            dungeonInstance = Instantiate(dungeonPrefab);
            dungeonInstance.transform.SetParent(transform);
            //Only update the nav mesh if needed.
            navSurface.BuildNavMesh();
        }

        //Find the spawnpoint gameobject
        Transform spawnPoint = dungeonInstance.transform.Find("SpawnPoint").transform;

        //If previously loaded
        if (instantiated) {
            dungeonInstance.SetActive(true);
            spawnPoint.position = lastPlayerLocation;
        }

        //Mostly used for the name of the dungeon
        //Enable the instance manager and join the instance.
        netManagerSettings.enabled = true;
        netManagerSettings.JoinInstance(dungeonSeed);

        LocalDungeonPlayerInstance = netManagerSettings.Instantiate(MainControl.Instance.DungeonPlayerPrefab, true, spawnPoint.position, spawnPoint.rotation);
        LocalDungeonPlayerInstance.transform.SetParent(transform);
        LocalDungeonPlayerInstance.name = "My " + LocalDungeonPlayerInstance.name;
        SetLocalPlayerControl(true);

        instantiated = true;

        CameraMovement cf = Camera.main.GetComponent<CameraMovement>();
        cf.currentTarget = LocalDungeonPlayerInstance.transform;

        Showing = true;
        Invoke("ReverseFade", fadeWaitTime);
    }

    //Once the dungeon is ready to go, and a small delay has expired, unfade the screen.
    void ReverseFade() {
        uif.onFadeCompleted = OnDungeonEnterFadeBackIn;
        uif.Reverse();
    }

    //The fade has finished, re-enable the player.
    void OnDungeonEnterFadeBackIn() {
        MainControl.SetLocalPlayerControl(true);
        Destroy(uif.gameObject);
    }

    public static void SetLocalPlayerControl ( bool canControl ) {
        if (CurrentInstance != null) {
            if (CurrentInstance.LocalDungeonPlayerInstance != null && CurrentInstance.LocalDungeonPlayerInstance.GetComponent<PhotonView>().IsMine) {
                GameObject pl = CurrentInstance.LocalDungeonPlayerInstance;
                pl.GetComponent<PlayerMovement>().enabled = canControl;
                pl.GetComponent<CombatController>().enabled = canControl;
                pl.GetComponent<NavMeshAgent>().enabled = canControl;//Enable the player movement script.
            }
        }
    }

}
