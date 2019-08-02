using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    private Map map;
    private Camera mainCamera;
    private Vector2 terrainPosition;

    public PlayerData data;

    Vector2[] currentPath;
    int pathIndex = 0;

    public GameObject pointerPrefab;
    private GameObject pointer;

    Vector3 lastTerrainPointClicked = new Vector3();
    IMapInteractable lastInteractableClicked;
    NavMeshAgent navMeshAgent;

    public float turnDistance = 5;
    public float speed = 1;
    public float turnSpeed = 5;
    public float turnDst = 5;
    public float stoppingDst = 10;

    public float heightSpeed = 1;
    Path path;

    public ActionConfirmation actionConfirmationGUI;

    void Start() {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        map = OverworldControl.Instance.Map;
        //Temporary stuff here.
        data = new PlayerData();
        data.Name = "Test player 1";
        data.gold = 10;
        
        pointer = Instantiate(pointerPrefab);
        pointer.SetActive(false);
        Invoke("moveUp", 0.5f);

        actionConfirmationGUI = MainControl.Instance.ActionConfirmationGUI.GetComponent<ActionConfirmation>();
        actionConfirmationGUI.OnResult += ActionConfirmed;
    }

    void moveUp() {
        terrainPosition = map.RealWorldToTerrainCoord(transform.position);
        transform.position = map.tileManager.GetTile(terrainPosition).position;
        
    }

    float resolution = 0.1f;

    float progress = 0;
    
    // Update is called once per frame
    public int state = 0;

    /** Since the state machine isn't fully sequential, I will do my best to map the states out here.
           -1 - a general error and reset state
            0 - waiting for the user to click on something on the map.

            //All about moving on the map
            from 0 to 5 - user has clicked on the terrain
            5 - waiting for pathfinding result
            from 5 to 6 - path was successfully found, move to state 6 (because this is incremented in a thread, it needs to be called from update)
            from 5 to -1 - no path was found return to waiting
            6 - show the action confirmation gui
            from 6 to 7 - action confirmation gui is showing
            7 - waiting for action confirmation gui response
            from 7 to 8 - action was confirmed
            from 7 to -1 - action was cancelled
            8 - moving player to targeted destination, waiting for path to complete
            from 7 to -1 - path was completed, resetting for next request

            //All about interacting with something
            from 0 to 10 - interactable was found, show action confirmation gui
            10 - waiting for interaction confirmation to close
            from 10 to 11 - action was confirmed, interacting with object
            from 10 to -1 - action was cancelled
            11 - waiting for object interaction to complete
            from 11 to -1 - interaction was completed, resetting for next request


     */


    void Update() {

        if (state == -1) {
            pointer.SetActive(false);
            if (lastInteractableClicked != null) {
                lastInteractableClicked.SetHighlight(false);
                lastInteractableClicked = null;
            }
            state = 0;
        }

        if (Input.GetButtonDown("Fire1")) {
            if (state == 0) {//Waiting for the user to choose what to do..
                LookForInteraction();
            }
        }

        switch (state) {
            case 6:
                actionConfirmationGUI.Show("Move Here", "Go", Input.mousePosition);
                state = 7;
            break;
            case 8:
                if (FollowPath()) {
                    //Path is complete!
                    pointer.SetActive(false);
                    terrainPosition = map.RealWorldToTerrainCoord(transform.position);
                    state = -1;//Go back to the waiting state.
                }
            break;
            case 11:
                //Waiting for the interaction to complete before allowing the player to move once more.
                if (lastInteractableClicked.InteractionComplete(this)) {
                    state = -1;
                    lastInteractableClicked.SetHighlight(false);
                    lastInteractableClicked = null;
                }
            break;
        }
    }

    public void LookForInteraction() {

        TerrainClick tc = TryClickTerrain(resolution, mainCamera, map);
        if (tc.terrainPoint != null) {//
            lastTerrainPointClicked = tc.terrainPoint.position;//A point on the terrain was found
            //Two options here, the end tile is occupied, in which case we attempt to interact, or it is not occupied in which case we navigate to that item
            if (tc.terrainPoint.Occupied) {//Attempt to interact with the object
                IMapInteractable interactable = tc.terrainPoint.occupyingObject.GetComponent<IMapInteractable>();//Interactable objects must inherit from IMapInteractable
                if (interactable != null && interactable.TryInteract(this).Interactable) {//if there is an interactable and it is currently interactable.
                    //Set this as the targeted object.
                    lastInteractableClicked = interactable;
                    interactable.SetHighlight(true);
                    actionConfirmationGUI.Show(interactable.GetActionName(), interactable.GetShortActionName(), Input.mousePosition);
                    state = 10;
                }
            } else if (tc.success) {
                //Attempt to pathfind.
                map.pathfinder.RequestPath(terrainPosition, tc.terrainPoint.gridPosition, map.GetPathfindingTileCost, map.TileIsPassable, true, RequestComplete, true);
                pointer.transform.position = lastTerrainPointClicked;
                pointer.SetActive(true);
                state = 5;
            } else {
                //No valid point was found???
                //Space is not occupied, and not passable
                //Just play an animation or something.
            }
        }//No point on the terrain was under the mouse if this happens
    }

    public void RequestComplete(PathResult result) {
        //This is technically state 5
        if (result.result) {
            //Path was found!
            currentPath = result.path;

            List<Vector3> waypoints = new List<Vector3>();
            foreach (Vector2 vec in result.path) {
                waypoints.Add(map.TerrainCoordToRealWorld(vec));
            }

            path = new Path(waypoints.ToArray(), waypoints[0], turnDistance, stoppingDst);
            pathIndex = 0;
            state = 6;
        } else {
            //Path was not found!!
            state = -1;
            
        }
    }

    public void ActionConfirmed(bool result) {
        //Action confirmation gui automatically closes itself.
        if (state == 7) {
            //Move somewhere
            if (result) {
                state = 8;
            } else {
                state = -1;
            }
        } else if (state == 10) {
            if (result) {
                //Interact with something
                lastInteractableClicked.Interact(this);
                state = 11;
            } else {
                state = -1;
            }
        }
        
    }

    public TerrainClick TryClickTerrain(float resolution, Camera camera, Map map) {
        if (!OverworldControl.Instance.GUIOpen) {
            Vector3 farpoint = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.farClipPlane));
            Vector3 closePoint = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
            if (map != null) {
                return map.TerrayCast(resolution, (farpoint - closePoint).normalized, camera.ScreenToWorldPoint(Input.mousePosition), camera.farClipPlane);
            }
        }
        TerrainClick tc = new TerrainClick();
        tc.success = false;
        return tc;
    }

    float lastY = 0;

    public bool FollowPath() {
        float speedPercent = 1;
        if (pathIndex == 0) {
            transform.LookAt(path.lookPoints[0]);
            lastY = transform.position.y;
        }

        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
        while (path.turnBoundaries [pathIndex].HasCrossedLine(pos2D)) {
            if (pathIndex == path.finishLineIndex) {
                return true;//Finished with path condition
            } else {
                pathIndex ++;
            }
        }
        if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
            speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / stoppingDst);
            if (speedPercent < 0.01f) {
                return true;
            }
        }
        Vector3 lp = new Vector3(path.lookPoints[pathIndex].x, 0, path.lookPoints[pathIndex].z);
        Quaternion targetRotation = Quaternion.LookRotation (lp - new Vector3(transform.position.x, 0, transform.position.z), Vector3.up);
        transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);

        //Make the vertical position work properly
        Tile t = OverworldControl.Instance.Map.tileManager.GetTile(OverworldControl.Instance.Map.RealWorldToTerrainCoord(transform.position));

        float diff = t.position.y - lastY;//Difference between here and destination
        diff = diff * Time.deltaTime * heightSpeed;
        lastY += diff;
        //lastY += 
        transform.position = new Vector3(transform.position.x, lastY, transform.position.z);
        return false;
    }

    public void OnDrawGizmos() {
        if (path != null) {
            path.DrawWithGizmos();
        }
    }

}