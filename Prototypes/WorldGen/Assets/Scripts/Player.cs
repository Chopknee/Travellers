using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.EventSystems;

namespace BaD.Modules.Terrain {
    public class Player: MonoBehaviourPunCallbacks {

        private Camera mainCamera;
        private Vector2 terrainPosition;

        public PlayerData Data { get; private set; }

        Vector2[] currentPath;
        int pathIndex = 0;

        public GameObject pointerPrefab;
        private GameObject pointer;

        Vector3 lastTerrainPointClicked = new Vector3();
        IMapInteractable lastInteractableClicked;

        public float turnDistance = 5;
        public float speed = 1;
        public float turnSpeed = 5;
        public float turnDst = 5;
        public float stoppingDst = 10;

        public float heightSpeed = 1;
        Path path;

        private ActionConfirmation actionConfirmationGUI;

        private Map map;
        [SerializeField]
#pragma warning disable 0649
        private LayerMask InteractionLayerMask;

        void Start () {
            if (!photonView.IsMine)
                return;

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            //Temporary stuff here.
            Data = new PlayerData(gameObject);
            Data.Name = PhotonNetwork.NickName;
            Data.gold = 10;

            pointer = Instantiate(pointerPrefab);
            pointer.SetActive(false);
            state = -100;//A completely unused state.
            actionConfirmationGUI = MainControl.Instance.ActionConfirmationUI.GetComponent<ActionConfirmation>();
            actionConfirmationGUI.OnResult += ActionConfirmed;

        }

        float resolution = 0.1f;

        //float progress = 0;

        // Update is called once per frame
        public int state = 0;

        /** Since the state machine isn't fully sequential, I will do my best to map the states out here.
               -1 - a general error and reset state
                0 - waiting for the user to click on something on the map.

                    All about moving on the map
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

                    All about interacting with something
                from 0 to 10 - interactable was found, show action confirmation gui
                10 - waiting for interaction confirmation to close
                from 10 to 11 - action was confirmed, interacting with object
                from 10 to -1 - action was cancelled
                11 - waiting for object interaction to complete
                from 11 to -1 - interaction was completed, resetting for next request

                15 - 20 is for moving closer to interactables
                Actions now need to deal with getting the player close enough
                    if the player is close enough skip the move part
                        else
                    tell the player they must move to the selected location (as close as possible)
                    then allow the player to interact

         */


        void Update () {
            if (!photonView.IsMine)
                return;

            if (state == -100) {//A state where the player has no actual control.
                //Wait until the map control has actually spawned in then we can move..
                if (OverworldControl.Instance != null && OverworldControl.Instance.MapReady) {
                    map = OverworldControl.Instance.Map;//Map is out for the issue
                    terrainPosition = map.RealWorldToTerrainCoord(transform.position);//This runs fine
                    transform.position = map.tileManager.GetTile(terrainPosition).position;
                    state = -1;
                }
                return;
            }

            if (state == -1) {
                Debug.Log("Player was put in reset mode.");
                pointer.SetActive(false);
                if (lastInteractableClicked != null) {
                    lastInteractableClicked.SetHighlight(false);
                    lastInteractableClicked = null;
                }
                state = 0;
            }

            if (Input.GetButtonDown("Interact")) {
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
                        terrainPosition = map.RealWorldToTerrainCoord(path.endPoint);
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
                case 16:
                    actionConfirmationGUI.Show("Move To " + lastInteractableClicked.GetDisplayName(), "Move", Input.mousePosition);
                    state = 17;
                    break;
                case 18:
                    if (FollowPath()) {
                        pointer.SetActive(false);
                        terrainPosition = map.RealWorldToTerrainCoord(path.endPoint);
                        lastInteractableClicked.SetHighlight(true);
                        actionConfirmationGUI.Show(lastInteractableClicked.GetActionName(), lastInteractableClicked.GetShortActionName(), Input.mousePosition);
                        state = 10;
                    }
                    break;
            }
        }

        public void LookForInteraction () {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }//Blocks out all clicks on UI components
            //Perform a raycast to see what the mouse was hovering over.
            RaycastHit rch = TryClickTerrain(resolution, mainCamera, map);
            //If there was an object hit
            if (rch.collider != null) {
                //Get the gameobject
                GameObject hitObject = rch.transform.gameObject;
                //Set the last point clicked
                lastTerrainPointClicked = rch.point;
                if (hitObject.tag == "Structure") {
                    IMapInteractable interactable = hitObject.GetComponent<IMapInteractable>();//Interactable objects must inherit from IMapInteractable
                    lastInteractableClicked = interactable;
                    if (interactable != null) {
                        InteractResult res = interactable.TryInteract(this);
                        if (res.Interactable) {
                            //Everything works out, do the thing
                            interactable.SetHighlight(true);
                            actionConfirmationGUI.Show(interactable.GetActionName(), interactable.GetShortActionName(), Input.mousePosition);
                            state = 10;
                        } else if (!res.Interactable && res.FailReason == InteractResult.Reason.TooFar) {
                            //Set up for a navigation
                            Vector2 pos = interactable.GetClosestPoint(this);
                            if (pos.x == -1) {
                                state = -1;
                                Debug.Log("Failed to find navigation for structure " + interactable.GetDisplayName());
                                return;
                            }
                            SetupNav(terrainPosition, pos);
                            lastTerrainPointClicked = pos;
                            pointer.transform.position = map.TerrainCoordToRealWorld(pos);
                            pointer.SetActive(true);
                            state = 15;
                        }
                    }
                } else if (hitObject.tag == "Map") {   
                    Vector2 terrainPoint = map.RealWorldToTerrainCoord(rch.point);
                    if (!map.tileManager.GetTile(terrainPoint).Blocked) {//First, get the tile and make sure we can pathfind to there
                        //Attempt to pathfind.
                        SetupNav(terrainPosition, terrainPoint);
                        lastTerrainPointClicked = terrainPoint;
                        pointer.transform.position = map.TerrainCoordToRealWorld(terrainPoint);
                        pointer.SetActive(true);
                        state = 5;
                    }
                }
            }//No object was under the mouse
        }

        public void PathFound ( PathResult result ) {
            //This is technically state 5
            Debug.Log("Pathfinding returned! " + state);
            if (result.result) {
                //Path was found!
                
                currentPath = result.path;

                List<Vector3> waypoints = new List<Vector3>();
                foreach (Vector2 vec in result.path) {
                    waypoints.Add(map.TerrainCoordToRealWorld(vec));
                }
                if (waypoints.Count > 0) {
                    path = new Path(waypoints.ToArray(), waypoints[0], turnDistance, stoppingDst);
                    pathIndex = 0;
                    if (state == 5) {
                        state = 6;
                    } else if (state == 15) {
                        state = 16;
                    }
                } else {
                    Debug.Log("Something went wrong while pathfinding.", this);
                }
            } else {
                //Path was not found!!
                Debug.Log("Could not find a path.");
                state = -1;

            }
        }

        public void ActionConfirmed ( bool result ) {
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
            } else if (state == 17) {
                if (result) {
                    state = 18;
                } else {
                    state = -1;
                }
            }

        }

        public RaycastHit TryClickTerrain ( float resolution, Camera camera, Map map ) {
            if (!OverworldControl.Instance.GUIOpen) {
                Vector3 farpoint = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.farClipPlane));
                Vector3 closePoint = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));
                Physics.Raycast(closePoint, ( farpoint - closePoint ).normalized, out RaycastHit hit, camera.farClipPlane, InteractionLayerMask, QueryTriggerInteraction.Ignore);
                return hit;
            }
            return new RaycastHit();//Returning an empty raycast
        }

        float lastY = 0;

        public bool FollowPath () {
            float speedPercent = 1;
            if (pathIndex == 0) {
                transform.LookAt(path.lookPoints[0]);
                lastY = transform.position.y;
            }

            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
                if (pathIndex == path.finishLineIndex) {
                    return true;//Finished with path condition
                } else {
                    pathIndex++;
                }
            }
            if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
                speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                if (speedPercent < 0.01f) {
                    return true;
                }
            }
            Vector3 lp = new Vector3(path.lookPoints[pathIndex].x, 0, path.lookPoints[pathIndex].z);
            Quaternion targetRotation = Quaternion.LookRotation(lp - new Vector3(transform.position.x, 0, transform.position.z), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);

            //Make the vertical position work properly
            Tile t = OverworldControl.Instance.Map.tileManager.GetTile(OverworldControl.Instance.Map.RealWorldToTerrainCoord(transform.position));

            float diff = t.position.y - lastY;//Difference between here and destination
            diff = diff * Time.deltaTime * heightSpeed;
            lastY += diff;
            //lastY += 
            transform.position = new Vector3(transform.position.x, lastY, transform.position.z);
            return false;
        }

        public void SetupNav(Vector2 start, Vector2 end) {
            map.pathfinder.RequestPath(start, end, map.GetPathfindingTileCost, map.TileIsPassable, true, PathFound, true);
        }

        public void OnDrawGizmos () {
            if (path != null) {
                path.DrawWithGizmos();
            }
        }

    }
}