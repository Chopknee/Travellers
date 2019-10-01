using BaD.Chopknee.Utilities;
using BaD.Modules;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetInstanceManager))]
public class DungeonManager: MonoBehaviour {

    public static DungeonManager CurrentInstance { get; private set; }
    
    public bool Showing { get; private set; }

    public GameObject localPlayerInstance { get; private set; }

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


    private bool instantiated = false;
    private NavMeshSurface navSurface;
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
    public void EnterArea() {

        MainControl.Instance.EnterArea(this);

        NetInstanceManager netManagerSettings = GetComponent<NetInstanceManager>();

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

        //Enables all gameobjects and shows the instance.
        if (!instantiated) {
            dungeonInstance = Instantiate(dungeonPrefab);
            instantiated = true;
            //Only update the nav mesh if needed.
            navSurface.BuildNavMesh();
        } else {
            dungeonInstance.SetActive(true);
        }

        CurrentInstance = this;

        //Move the player to the spawn of the dungeon
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        dungeonSeed = Choptilities.Vector3ToID(transform.position);

        //Enable the instance manager and join the instance.
        netManagerSettings.enabled = true;
        netManagerSettings.JoinInstance(dungeonSeed);

        MainControl.Instance.DungeonPlayerPrefab.GetComponent<PlayerMovement>().enabled = false;//Stop this script from causing issues
        localPlayerInstance = netManagerSettings.Instantiate(MainControl.Instance.DungeonPlayerPrefab, true, spawnPoint.transform.position, spawnPoint.transform.rotation);
        localPlayerInstance.GetComponent<PlayerMovement>().enabled = true;//Enable the player movement script.
        localPlayerInstance.transform.SetParent(transform);

        CameraMovement cf = Camera.main.GetComponent<CameraMovement>();
        cf.currentTarget = localPlayerInstance.transform;
        cf.smoothness = 5;
        cf.pan = 100;
        cf.turnSpeed = 2;
        cf.offset = 2;
        cf.verticalOffset = 4;
        cf.zoomSensitivity = 0.2f;
        cf.distanceToPlayer = 5;
        cf.horizontalDistanceToPlayer = 3;
        cf.mouseSensitivity = 1.5f;
        Showing = true;
    }

    public void ExitArea() {
        MainControl.Instance.ExitArea(this);
        HideArea();
    }

    //Exits the area without running the world exit area code.
    public void HideArea() {
        //Hides all gameobjects and disables the instance
        dungeonInstance.SetActive(true);

        //PhotonNetwork.Destroy(playerInstance);
        NetInstanceManager netManager = GetComponent<NetInstanceManager>();
        netManager.DestroyObject(localPlayerInstance);
        //Shut down the instance manager.
        netManager.LeaveInstance();
        netManager.enabled = false;
        navSurface.enabled = false;

        CurrentInstance = null;
        Showing = false;
    }


}
