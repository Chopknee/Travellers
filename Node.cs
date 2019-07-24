using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public bool walkable;
    public bool dungeonTile;
    public Vector3 worldPosition;
    public int gCost, hCost;
    public int gridX, gridY;
    public Node parent;
    public GameObject room;
    
    public Node(bool _walkable, Vector3 _worldPos, int gridX, int gridY, int direction)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
        
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

}
