using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUse : MonoBehaviour
{
    //A sample grid
    public AGridNode[,] grid;
    int Width = 100;
    int Height = 100;

    void Start() {
        grid = new AGridNode[Width, Height];
        //Generating the example grid
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                grid[x,y] = new AGridNode();
            }
        }

        Invoke("DoPathfind", 1);
    }

    Vector2[] tilePath;
    public void DoPathfind() {
        //Actually finding a path
        PathfindingGrid pg = new PathfindingGrid(Width, Height);
        pg.RequestPath(new Vector2(10, 20), new Vector2(85, 50), TileMoveCost, TileIsPassable, true, Complete);
    }

    public void OnDrawGizmos () {
        //Rendering the path if it is available
        if (tilePath != null) {
            foreach (Vector2 t in tilePath) {
                Gizmos.DrawCube(t, Vector3.one);
            }
        }
    }
    
    //Here you might reference a secondary grid that holds the information about tiles.
    public bool TileIsPassable(Vector2 position) {
        
        return grid[(int) position.x, (int) position.y].passable;
    }

    //Here is where an additional movement penalty can be implemented, for normal pathfinding, just use the newPosition node.
    //Both current position and new position are included because I needed them to calculate change in elevation
    public int TileMoveCost(Vector2 currentPosition, Vector2 newPosition) {
        
        return grid[(int) newPosition.x, (int) newPosition.y].cost;
    }

    public void Complete(PathResult result) {
        if (result.result) {
            tilePath = result.path;
        } else {
            tilePath = null;
        }
    }

    //A simple grid node class that holds a random movement penalty and the passable variable
    public class AGridNode {
        public int cost = Random.Range(0, 1000);
        public bool passable = true;
    }
}
