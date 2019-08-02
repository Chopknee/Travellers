using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;


    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                bool start = false, end = false;
                
                grid[x, y] = new Node(walkable, worldPoint, x, y, 0, start, end); // GENERATING NODE HERE, default direction of north.

            }
        }
    }

    //check all 4 nodes around our current node (passed into the function) and add them to our neighbors list, then return the list.

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0 || SkipCorners(x, y))
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public void GenerateWalls()
    {
        RoomTemplates rooms = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        List<Node> unwalkables = new List<Node>();

        foreach (Node n in grid)
        {
            if (n.room == null)
            {
                n.room = Instantiate(rooms.wall, n.worldPosition, Quaternion.identity);
                n.dungeonTile = true;
            }
           
        }
    }
    public Node CheckNode(Node node, int index)
    {
        int cX = node.gridX;
        int cY = node.gridY;
        switch (index) // this checks left, right, top, or bottom based on the index... I pass in currentNode (from the grid)
        {
            case 0:
                //north
                if (grid[cX, cY + 1] != null)
                    return grid[cX, cY + 1];
                break;
            case 1:
                //east
                if (grid[cX + 1, cY] != null)
                    return grid[cX, cY + 1];
                break;
            case 2:
                //south
                if (grid[cX, cY - 1] != null)
                    return grid[cX, cY + 1];
                break;
            case 3:
                //west
                if (grid[cX - 1, cY] != null)
                    return grid[cX, cY + 1];
                break;
        }
        return null;
    }


    bool SkipCorners(int x, int y)
    {
        return (!(x == 0 || y == 0));
    }

    

    public Node GetNodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path = new List<Node>();

    public bool showGizmos = false;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (!showGizmos) return;
        RoomTemplates rooms = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();



        if (grid != null)
        {
            Node playerNode = GetNodeFromWorldPoint(player.position);

            foreach (Node n in grid)
            {

                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                

                if (playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.blue;
                        
                    }
                }
                float s = (nodeDiameter - 0.1f);
                Gizmos.DrawCube(n.worldPosition, new Vector3(s, 1, s));
            }
        }
    }

    

}





//return ((x == -1 && y == -1)     // bottom left corner
//    ||  (x ==  1 && y == -1)     // bottom right corner
//    ||  (x == -1 && y ==  1)     // top left corner
//    ||  (x ==  1 && y ==  1));   // top right corner


#region old
/*
public Transform startPos, endPos;
public LayerMask wallMask;
public Vector2 gridWorldSize;
public float nodeRadius;
public float distance;

Node[,] grid;
public List<Node> finalPath;

float nodeDiameter;
int gridSizeX, gridSizeY;

private void Start()
{
    nodeDiameter = nodeRadius * 2;
    gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
    gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    CreateGrid();
}

void CreateGrid()
{
    grid = new Node[gridSizeX, gridSizeY];

    Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

    for (int x = 0; x < gridSizeX; x++)
    {
        for (int y = 0; y < gridSizeY; y++)
        {

            Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
            bool wall = true; //make the node a wall

            //If the node is not being obstructed
            //Quick collision check against the current node and anything in the world at its position. If it is colliding with an object with a WallMask,
            //The if statement will return false.
            if (Physics.CheckSphere(worldPoint, nodeRadius, wallMask))
            {
                wall = false;
            }
            grid[x, y] = new Node(wall, worldPoint, x, y);
        }
    }
}


public List<Node> GetNeighbors(Node i)
{
    List<Node> neighbors = new List<Node>();
    int iCheckX, iCheckY;


    //right side
    iCheckX = i.gridX + 1;
    iCheckY = i.gridY;
    if (iCheckX >= 0 && iCheckX < gridSizeX)
    {
        if (iCheckY >= 0 && iCheckY < gridSizeY)
        {
            neighbors.Add(grid[iCheckX, iCheckY]);
        }
    }

    //left side
    iCheckX = i.gridX - 1;
    iCheckY = i.gridY;
    if (iCheckX >= 0 && iCheckX < gridSizeX)
    {
        if (iCheckY >= 0 && iCheckY < gridSizeY)
        {
            neighbors.Add(grid[iCheckX, iCheckY]);
        }
    }

    // top
    iCheckX = i.gridX;
    iCheckY = i.gridY + 1;


    if (iCheckX >= 0 && iCheckX < gridSizeX)
    {
        if (iCheckY >= 0 && iCheckY < gridSizeY)
        {
            neighbors.Add(grid[iCheckX, iCheckY]);
        }
    }

    //bottom
    iCheckX = i.gridX;
    iCheckY = i.gridY - 1;


    if (iCheckX >= 0 && iCheckX < gridSizeX)
    {
        if (iCheckY >= 0 && iCheckY < gridSizeY)
        {
            neighbors.Add(grid[iCheckX, iCheckY]);
        }
    }

    return neighbors;

}

public Node NodeFromWorldPosition(Vector3 worldPos)
{
    float xPoint = ((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
    float yPoint = ((worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y);

    xPoint = Mathf.Clamp01(xPoint);
    yPoint = Mathf.Clamp01(yPoint);

    int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
    int y = Mathf.RoundToInt((gridSizeY - 1) * yPoint);
    return grid[x, y];
}
private void OnDrawGizmos()
{
    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
    if (grid != null)
    {
        foreach (Node node in grid)
        {
            if (node.isWall)
            {
                Gizmos.color = Color.white;
            }
            else Gizmos.color = Color.yellow;

            if (finalPath != null)
            {
                if (finalPath.Contains(node))
                    Gizmos.color = Color.red;
            }

            Gizmos.DrawCube(node.Position, Vector3.one * (nodeDiameter - distance));
        }
    }
}*/
#endregion