using BaD.Chopknee.Utilities;
using BaD.Modules;
using BaD.Modules.Networking;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NetInstanceManager))]
public class DungeonManager: MonoBehaviour {

    public static DungeonManager CurrentInstance { get; private set; }

    [Header("Tile Informations")]
    [SerializeField]
    [Tooltip("Prefab for the start tile. This prefab needs a gameobject with the tag SpawnPoint on it.")]
#pragma warning disable 0649
    private GameObject StartTile;
    [SerializeField]
    [Tooltip("The 'final boss' or exit tile of the instance. May be omitted if the start tile supports both entry and exit.")]
#pragma warning disable 0649
    private GameObject EndTile;
    [SerializeField]
    [Tooltip("These are more or less plain tiles. Good for paths.")]
#pragma warning disable 0649
    private GameObject[] BasicTiles;
    [Tooltip("At the moment, unused.")]
    public bool SpawnSpecialTilesOnce = true;
    [SerializeField]
    [Tooltip("Tiles that have special events. Put tiles in order of how deep in the dungeon they should generate. Each tile in the list gets spawned once, so stack them if you want multiple instances.")]
#pragma warning disable 0649
    private GameObject[] SpecialTiles;
    [SerializeField]
    [Tooltip("Tiles used for the impassable areas of the instace.")]
#pragma warning disable 0649
    private GameObject[] ImpassableTiles;
    [Tooltip("Automatically align the border tiles to the specific edge of the grid.")]
    public bool AlignBorderTilesToEdge;
    [SerializeField]
    [Tooltip("Border tiles are spawned on the 4 edges of the instance.")]
#pragma warning disable 0649
    private GameObject[] BorderTiles;
    [Header("Nav Mesh Setting")]
    [Tooltip("The height setting of the nav mesh volume. The width and length are automatically calculated by the size of the dungeon.")]
    public float navMeshHeight;
    public LayerMask navMeshLayers;

    [Header("Generator Settings")]
    [Tooltip("If set, the generator starts the path from the center of the grid. Otherwise the start position is selected on a random edge.")]
    public bool StartPathFromCenter;
    [Tooltip("The seed used for the generator.")]
    public int GeneratorSeed = 10;
    [Tooltip("How big the grid for generating the instance should be.")]
    public Vector2 GridSize;
    [Tooltip("The size of each tile in the instance.")]
    public Vector2 GridScale;
    [Tooltip("How many times to run the pathing function. Larger numbers means more chances for branching.")]
    public int PathNodeCount;
    [Tooltip("Generally how long the distance between nodes will be.")]
    public float HallLengths;
    [Tooltip("The range of how much a node will turn before generating the next hall. Unit is in radians, so Mathf.PI = half a circle.")]
    [Range(0, Mathf.PI*2)]
    public float NodeTwist;
    [Tooltip("The chance of a branch to spawn off the main branch.")]
    [Range(0, 1)]
    public float BranchChance;
    [Tooltip("The chance of a branch ending before the maximum allowed number of branch nodes has been reached.")]
    [Range(0, 1)]
    public float BranchDeathChance;
    [Tooltip("The maximum number of nodes a branch is allowed to generate.")]
    public int BranchMaximumNodes;

    public bool Showing { get; private set; }

    public GameObject playerInstance { get; private set; }

    //Private fields
    List<GameObject> areaObjects = new List<GameObject>();

    //Remainder of these are for generating and other purposes
    List<Branch> branches = new List<Branch>();
    Vector2 startPosition;
    private PathfindingGrid pathfinder;
    private bool generated = false;
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
        //Making a clone to prevent the current an varied issues with using the direct structure object.
        //if (NetManager == null) {
        //    NetManager = new GameObject("Instance Manager " + GeneratorSeed);
        //    NetInstanceManager dmim = NetManager.AddComponent<NetInstanceManager>();
        //    NetInstanceManager.CloneSettings(netManagerSettings, dmim);
        //    netManagerSettings = dmim;
        //    netManagerSettings.Awake();//
        //}

        //Generate the nav mesh - add the nav mesh component if it is not already present.
        if (navSurface == null) {
            navSurface = GetComponent<NavMeshSurface>();
            if (navSurface == null) {
                navSurface = gameObject.AddComponent<NavMeshSurface>();
                navSurface.collectObjects = CollectObjects.Volume;
                navSurface.size = new Vector3(GridSize.x * GridScale.x, navMeshHeight, GridSize.y * GridScale.y);
                navSurface.center = new Vector3((GridSize.x * GridScale.x)/2-(GridScale.x/2), 0, (GridSize.y * GridScale.y)/2-(GridScale.y/2));
                navSurface.overrideTileSize = true;
                navSurface.voxelSize = 0.126667f;
                navSurface.overrideTileSize = true;
                navSurface.tileSize = 32;
                navSurface.layerMask = navMeshLayers;
            } else {
                navSurface.enabled = true;
            }
        }

        //Enables all gameobjects and shows the instance.
        if (!generated) {
            Generate();
            generated = true;
            //Only update the nav mesh if needed.
            navSurface.BuildNavMesh();
        } else {
            foreach (GameObject go in areaObjects) {
                go.SetActive(true);
            }
        }

        CurrentInstance = this;

        //Move the player to the spawn of the dungeon
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        //Enable the instance manager and join the instance.
        netManagerSettings.enabled = true;
        netManagerSettings.JoinInstance(GeneratorSeed);

        MainControl.Instance.DungeonPlayerPrefab.GetComponent<PlayerMovement>().enabled = false;//Stop this script from causing issues
        playerInstance = netManagerSettings.Instantiate(MainControl.Instance.DungeonPlayerPrefab, true, spawnPoint.transform.position, spawnPoint.transform.rotation);
        playerInstance.GetComponent<PlayerMovement>().enabled = true;//Enable the player movement script.
        playerInstance.transform.SetParent(transform);

        CameraMovement cf = Camera.main.GetComponent<CameraMovement>();
        cf.currentTarget = playerInstance.transform;
        cf.smoothness = 5;
        cf.pan = 100;
        cf.turnSpeed = 2;
        cf.offset = 2;
        cf.verticalOffset = 4;
        cf.zoomSensitivity = 0.2f;
        cf.distanceToPlayer = 5;
        cf.horizontalDistanceToPlayer = 3;
        cf.mouseSensitivity = 1.5f;
        //cf.verticalLimits = new Vector2(0.5f, 8);
        //cf.offsetLimits = new Vector2(0.5f, 4);
        Showing = true;
    }

    public void ExitArea() {
        MainControl.Instance.ExitArea(this);
        HideArea();
    }

    //Exits the area without running the world exit area code.
    public void HideArea() {
        //Hides all gameobjects and disables the instance
        foreach (GameObject go in areaObjects) {
            go.SetActive(false);
        }

        //PhotonNetwork.Destroy(playerInstance);
        NetInstanceManager netManager = GetComponent<NetInstanceManager>();
        netManager.DestroyObject(playerInstance);
        //Shut down the instance manager.
        netManager.LeaveInstance();
        netManager.enabled = false;
        navSurface.enabled = false;

        CurrentInstance = null;
        Showing = false;
    }

    private void Generate () {
        allNodes.Clear();
        Noise.Reset(GeneratorSeed);
        branches = new List<Branch>();
        Choptilities.DestroyList(areaObjects);
        areaObjects = new List<GameObject>();
        MakeDungeon();
    }

    private void MakeDungeon () {
        //This function gets the ball rolling.
        GridSize = new Vector2(Mathf.RoundToInt(GridSize.x), Mathf.RoundToInt(GridSize.y));
        //Two options here, either make an east-west, or north-south dungeon

        float dir = Noise.GetRandomNumber(GeneratorSeed) * Mathf.PI*2;
        Vector2 start = new Vector2(GridSize.x/2, GridSize.y/2);

        if (!StartPathFromCenter) {
            bool plane = Noise.GetRandomNumber(GeneratorSeed) > 0.5f;
            bool orientation = Noise.GetRandomNumber(GeneratorSeed) > 0.5f;
            if (plane) {
                //Do north-south
                start.x = Noise.GetRandomRange(GeneratorSeed, 0, (int) GridSize.x - 1);
                start.y = ( orientation ) ? 0 : GridSize.y - 1;
                dir = ( orientation ) ? 0 : Mathf.PI;
            } else {
                //Do east-west
                start.x = ( orientation ) ? 0 : GridSize.x - 1;
                start.y = Noise.GetRandomRange(GeneratorSeed, 0, (int) GridSize.y - 1);
                dir = ( orientation ) ? Mathf.PI * 1.5f : Mathf.PI * 0.5f;
            }
        }
        startPosition = start;
        allNodes.Add(start);
        pathfinder = new PathfindingGrid(Mathf.RoundToInt(GridSize.x), Mathf.RoundToInt(GridSize.y));
        branches.Add(new Branch(pathfinder, start, this, true, 0, PathNodeCount, GeneratorSeed+1, dir));
        branches[0].MakePathlet();
        GenerateStructures();
    }

    List<Vector2> allNodes = new List<Vector2>();

    private void GenerateStructures () {
        //Take the branches and generated paths, and convert them to a single list
        int roomsSeed = GeneratorSeed + 2 + branches.Count;
        foreach (Branch br in branches) {
            foreach (Vector2 position in br.nodes) {
                if (!allNodes.Contains(position)) {
                    allNodes.Add(position);
                }
            }
        }

        //Build the impassable areas
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                if (!allNodes.Contains(new Vector2(x, y))) {
                    Vector3 pos = new Vector3(x * GridScale.x, 0, y * GridScale.y);
                    //spawn an impasse gameobject
                    areaObjects.Add(InstantiateRandom(ImpassableTiles, roomsSeed, pos, Quaternion.identity));
                }
            }
        }

        ////Build the borders
        for (int x = 0; x < GridSize.x; x++) {
            //Do the horizontal borders
            Vector3 pos = new Vector3(x * GridScale.x, 0, -1 * GridScale.y);//Spawning 1 below the bottom?
            areaObjects.Add(InstantiateRandom(BorderTiles, roomsSeed, pos, Quaternion.identity));
            pos = new Vector3(x * GridScale.x, 0, ( GridSize.y ) * GridScale.y);//Spawning 1 above the top
            areaObjects.Add(InstantiateRandom(BorderTiles, roomsSeed, pos, Quaternion.identity));

        }
        for (int y = -1; y < GridSize.y + 1; y++) {
            //Do the vertical borders
            Vector3 pos = new Vector3(-1 * GridScale.x, 0, y * GridScale.y);//Spawning 1 above the top
            areaObjects.Add(InstantiateRandom(BorderTiles, roomsSeed, pos, Quaternion.identity));
            pos = new Vector3(( GridSize.x ) * GridScale.x, 0, y * GridScale.y);//Spawning 1 above the top
            areaObjects.Add(InstantiateRandom(BorderTiles, roomsSeed, pos, Quaternion.identity));
        }

        //Build the filler that is the dungeon itself
        int pathNumber = 0;
        float progress = 0;
        int stride = Mathf.Max(Mathf.FloorToInt((allNodes.Count - 1.0f) / (SpecialTiles.Length + 0.0f)), 1);
        List<GameObject> specialTilePrefabs = new List<GameObject>();
        specialTilePrefabs.AddRange(SpecialTiles);

        foreach (Vector2 node in allNodes) {
            GameObject prefabToSpawn;
            if (node == startPosition) {
                prefabToSpawn = StartTile;
            } else if (pathNumber == allNodes.Count - 1) {
                prefabToSpawn = EndTile;
            } else {
                int index = Noise.GetRandomRange(roomsSeed, 0, BasicTiles.Length);
                prefabToSpawn = BasicTiles[index];
                if (SpawnSpecialTilesOnce) {
                    if (pathNumber % stride == 0) {
                        //Spawn a special
                        if (specialTilePrefabs.Count > 0) {
                            prefabToSpawn = specialTilePrefabs[0];
                            specialTilePrefabs.RemoveAt(0);
                        }
                    }
                    //Distribute among the central tiles evenly
                } else {

                }
                
            } //Later, will determine if should spawn a special tile.

            GameObject go = Instantiate(prefabToSpawn);
            go.transform.position = new Vector3(node.x * GridScale.x, 0, node.y * GridScale.y);
            go.transform.SetParent(transform);
            areaObjects.Add(go);
            pathNumber++;
            progress = (0.0f + pathNumber) / (0.0f + allNodes.Count);
        }
    }

    private Vector2 GetRandomGridPoint(int seed) {
        return new Vector2(Noise.GetRandomRange(seed, 0, GridSize.x-1), Noise.GetRandomRange(seed, 0, GridSize.y-1));
    }

    private Vector2 GetNextRandomPoint(Vector2 start, bool allowGobacks,float lastDirection, out float newDirection, int seed) {
        bool gotten = false;
        Vector2 candidate = Vector2.zero;
        newDirection = lastDirection;
        float minTwist = ( NodeTwist == 0 ) ? Mathf.PI * 2 : NodeTwist;
        float rand = Noise.GetRandomRange(seed, -minTwist / 2f, minTwist / 2f);
        while (gotten != true) {
            newDirection = newDirection + rand;
            Vector2 dir = new Vector2(Mathf.Sin(newDirection), Mathf.Cos(newDirection));
            candidate = start + dir * Mathf.Max(1, HallLengths);
            candidate = new Vector2(Mathf.RoundToInt(candidate.x), Mathf.RoundToInt(candidate.y));

            if (candidate.x > 0 && candidate.x < GridSize.x && candidate.y > 0 && candidate.y < GridSize.y) {
                gotten = ( candidate == start && allowGobacks || !allowGobacks && candidate != start );
            }
        }
        return candidate;
    }

    private GameObject InstantiateRandom(GameObject[] prefabs, int seed, Vector3 position, Quaternion rotation) {
        int ind = Noise.GetRandomRange(seed, 0, prefabs.Length);
        GameObject go = Instantiate(prefabs[ind]);
        go.transform.SetParent(transform);
        go.transform.position = position;
        go.transform.rotation = rotation;
        return go;
    }

    private class Branch {
        PathfindingGrid pathfinder;
        public List<Vector2> nodes;
        private float deathChance;
        public bool finished;
        private int maxNodes;
        private int nodeNumber;
        private bool isMainBranch;
        private DungeonManager dm;
        float lastDirection = 0;
        int seedNumber = 0;

        Vector2 startPosition;

        public Branch(PathfindingGrid pathfinder, Vector2 start, DungeonManager dm, bool isMainBranch, float deathChance, int maxNodes, int seedNumber, float startDirection) {
            Noise.Reset(seedNumber);
            nodes = new List<Vector2>();
            this.pathfinder = pathfinder;
            this.dm = dm;
            this.deathChance = deathChance;
            this.maxNodes = maxNodes;
            this.isMainBranch = isMainBranch;
            lastDirection = startDirection;
            startPosition = start;
            this.seedNumber = seedNumber;
        }

        public void MakePathlet () {
            if (!isMainBranch) {
                //Chance of branch dying
                float death = Noise.GetRandomNumber(seedNumber);
                if (death < deathChance) {
                    return;//This branch is dead now.
                }
            }

            if (nodeNumber < maxNodes) {
                nodeNumber++;
                //Generate the end point
                Vector2 endpoint = dm.GetNextRandomPoint(startPosition, false, lastDirection, out lastDirection, seedNumber);
                //For making extra branches
                if (isMainBranch) {
                    float branch = Noise.GetRandomNumber(seedNumber);
                    if (branch < dm.BranchChance) {
                        lock (dm.branches) {

                            int newSeed = seedNumber + 1 + dm.branches.Count;
                            float branchDir = Noise.GetRandomRange(newSeed, -Mathf.PI / 4, Mathf.PI / 4);

                            Branch br = new Branch(pathfinder, startPosition, dm, false, dm.BranchDeathChance, dm.BranchMaximumNodes, newSeed, branchDir);
                            dm.branches.Add(br);
                            br.MakePathlet();
                        }
                    }
                }

                PathResult res = pathfinder.RequestPath(startPosition, endpoint, TileCost, TilePassable, false, false);
                startPosition = endpoint;
                if (res.result) {
                    nodes.AddRange(res.path);
                    MakePathlet();
                }//If the path failed, kill the branch
            }//The path was finished
        }

        private int TileCost ( Vector2 currentGridPosition, Vector2 nextGridPosition ) {
            return 0;
        }

        bool TilePassable ( Vector2 position ) {
            return true;
        }
    }
}
