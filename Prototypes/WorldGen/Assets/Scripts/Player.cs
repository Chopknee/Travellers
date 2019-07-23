using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Player : MonoBehaviour
{

    private Map map;
    private Camera mainCamera;
    private Vector2 terrainPosition;

    volatile bool pathRequested = false;
    volatile bool followPath = false;
    Vector2[] currentPath;
    int pathIndex = 0;

    void Start() {
        Invoke("start", 1);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void start() {
        map = Control.Instance.Map;
        Tile occupyingTile = map.tileManager.GetTile(map.RealWorldToTerrainCoord(transform.position));
        if (occupyingTile != null) {
            transform.position = occupyingTile.position + new Vector3(0, 1, 0);
            terrainPosition = occupyingTile.gridPosition;
        }

    }

    float resolution = 0.1f;

    float progress = 0;
    public float speed = 1;
    // Update is called once per frame
    void Update() {

        if (Input.GetButtonDown("Fire1") && !pathRequested) {
            TerrainClick tc = TryClickTerrain(resolution, mainCamera, map);
            tp = tc.terrainPoint.position;
            if (tc.success) {
                //Attempt to pathfind.
                
                map.pathfinder.RequestPath(terrainPosition, tc.terrainPoint.gridPosition, map.GetPathfindingTileCost, map.TileIsPassable, true, RequestComplete);
                pathRequested = true;
                
            }
        }

        if (followPath) {
            progress += Time.deltaTime * speed;
            //Moving from one point to another
            Vector2 tp = Vector2.Lerp(terrainPosition, currentPath[pathIndex], progress);
            //UnityEngine.Debug.Log("Pathfinding! " + progress);
            transform.position = map.TerrainCoordToRealWorld(tp);
            if (progress > 1) {
                progress = 0;
                terrainPosition = currentPath[pathIndex];
                pathIndex ++;
                if (pathIndex == currentPath.Length) {
                    followPath = false;
                }
            }
        }
    }

    Vector3 tp = new Vector3();

    public void OnDrawGizmos() {
        if (Application.isPlaying) {
            if (currentPath != null) {
                Gizmos.color = new Color(0, 255, 255);
                Gizmos.DrawCube(currentPath[currentPath.Length-1], Vector3.one*10);
            }
            Gizmos.color = new Color(255, 0, 255);
            Gizmos.DrawCube(tp, Vector3.one*10);
        }
    }

    //This is called from a thread
    public void RequestComplete(PathResult result) {
        pathRequested = false;
        //Pathfind here.
        if (result.result) {
            currentPath = result.path;
            followPath = true;
            pathIndex = 0;
        }
    }

    public TerrainClick TryClickTerrain(float resolution, Camera camera, Map map) {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        TerrainClick res = new TerrainClick();
        res.success = false;
        float lastY = 0;
        //Loop through all the points between the camera near and camera far divided into the resolution
        for (float i = 0; i < mainCamera.farClipPlane; i+=resolution) {
            Vector2 mousePos = Input.mousePosition;
            Vector3 mouseWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, i));
            Tile t = map.tileManager.GetTile(map.RealWorldToTerrainCoord(mouseWorldPoint));
            if (t != null) {
                if (i == 0) 
                    lastY = mouseWorldPoint.y;

                if (Mathf.Abs(t.position.y - lastY) > Mathf.Abs(t.position.y - mouseWorldPoint.y)) {
                    lastY = mouseWorldPoint.y;
                    res.success = true;
                    res.terrainPoint = t;
                }

                if (mouseWorldPoint.y <= t.position.y) {
                    break;//No sense in continuing on if the ray goes through the terrain
                }
            }
        }
        sw.Stop();
        //UnityEngine.Debug.Log("Raycast complete, took " + (sw.ElapsedMilliseconds/1000f) + " seconds.");
        return res;
    }

    public struct TerrainClick {
        public bool success;
        public Tile terrainPoint;
    }
}
