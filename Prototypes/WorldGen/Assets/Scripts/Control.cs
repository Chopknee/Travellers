using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class Control: MonoBehaviour {
    
    public GameObject ToObject;
    public GameObject FromObject;

    public static Control Instance {
        get {
            return inst;
        }
    }

    private static Control inst;
    
    private Map map;
    public Map Map {
        get {
            return map;
        }
    }

    private void Awake () {
        inst = this;
    }

    void Start () {
        map = GetComponent<Map>();
        map.Generate();
        Invoke("StartPathfinding", 1);
        oldTo = Vector2.zero;
        oldFrom = Vector2.zero;
    }

    bool canRequest = false;

    void Update() {
        MakeRequest();
    }


    public float heightPenalty = 10;
    //public volatile List<Tile> tilePath = new List<Tile>();
    public ConcurrentStack<Tile> tileStack = new ConcurrentStack<Tile>();

    public void OnDrawGizmos () {
        if (tileStack != null) {
            foreach (Tile t in tileStack) {
                Gizmos.DrawCube(t.position, Vector3.one);
            }
        }
    }

    public void StartPathfinding() {
        canRequest = true;
    }

    Vector2 oldTo;
    Vector2 oldFrom;

    public void MakeRequest() {
        Vector2 from = new Vector2(200, 50);
        Vector2 to = new Vector2(0, 0);
        if (ToObject != null && FromObject != null) {
            from = map.RealWorldToTerrainCoord(FromObject.transform.position);
            to = map.RealWorldToTerrainCoord(ToObject.transform.position);
            if (from != oldFrom || to != oldTo) {
                oldTo = to;
                oldFrom = from;
                if (canRequest) {
                    PathfindingGrid newGrid = new PathfindingGrid(map.mapChunkSize, map.mapChunkSize);
                    newGrid.RequestPath(from, to, GetTileCost, TileIsPassable, true, RequestComplete);
                    canRequest = false;
                    tileStack.Clear();
                }
            }
        }
    }

    public int GetTileCost(Vector2 currentPosition, Vector2 nextPosition) {
        return (int)Mathf.Max(0, ((map.GetHeight(nextPosition) - map.GetHeight(currentPosition)) * heightPenalty));
    }

    public bool TileIsPassable(Vector2 position) {
        return map.tileManager.GetTile(position).CanWalkHere();//For now, all tiles will be marked as passable.
    }

    public void RequestComplete(PathResult result) {
        if (tileStack == null) { tileStack.Clear(); }
        if (result.result) {
            foreach (Vector2 position in result.path) {
                Tile t = map.tileManager.GetTile(position);
                if (t != null)
                    tileStack.Push(t);
            }
        }
        canRequest = true;
    }
}
