using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    public Transform seeker, target1, target2;
    RoomTemplates rooms;
    public NavMeshSurface surface;
    public int currentRoomCount = 0;
    private int requestedRoomMinimum = 5;
    bool retracingPath;
    bool end;
    float r1, r2;
    Transform player;

    private void Awake()
    {
        
        r1 = Mathf.Round(Random.Range(3, 6));
        r2 = Mathf.Round(Random.Range(5, 9));
        grid = GetComponent<Grid>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rooms = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("UpdateNavigation", .5f);
    }



    private void Update()
    {
        FindPath(target1.position, target2.position);
        
        FindPath(target1.position, seeker.position);
        end = true;
        grid.GenerateWalls();
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);
        

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        startNode.start = true;
        targetNode.end = true;
        
        
        

        while (openSet.Count > 0)
        {
            currentRoomCount = openSet.Count;
            Node currentNode = openSet[0];

            
            

            for (int i = 1; i < openSet.Count; i++)
            {

                float r = Mathf.Clamp(Random.Range(1, closedSet.Count), 1, 2);
                //Random.InitState(100);
                if ((Random.Range(1, 2 * r)) < 2 && openSet.Count > r1 && openSet.Count < r2 && Time.time < .5f)
                {
                    if (openSet[i].fCost >= currentNode.fCost)
                    {
                        currentNode = openSet[i];
                    }

                }
                else if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost))
                {
                    currentNode = openSet[i];
                }

                else if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode && !retracingPath)
            {
                retracingPath = true;
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

            

        }
    }


    void RetracePath(Node startNode, Node endNode)
    {
        if (end) return;
        retracingPath = true;
        List<Node> path = new List<Node>();
        List<Node> nodeNeighbors = new List<Node>();
        Node currentNode = endNode;



        while (currentNode != startNode)
        {
            path.Add(currentNode);
            nodeNeighbors = grid.GetNeighbors(currentNode);

            // 0 means north, ours needs south
            // 1 means east, ours needs west
            // 2 means south, ours needs north
            // 3 means west, ours needs east

            if (currentNode.dungeonTile == false)
            {
                //destroy the current gameobject that currentnode is referencing
                Destroy(currentNode.room);
                

                currentNode.room = Instantiate(rooms.rooms[Random.Range(0, rooms.rooms.Length)], currentNode.worldPosition, Quaternion.identity);


                currentNode.dungeonTile = true;
            }

            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;

        Destroy(startNode.room);
        Destroy(endNode.room);

        startNode.room = Instantiate(rooms.startRoom, startNode.worldPosition, Quaternion.identity);
        endNode.room = Instantiate(rooms.endRoom, endNode.worldPosition, Quaternion.identity);
        player.position = new Vector3(startNode.worldPosition.x + 2f, player.position.y, startNode.worldPosition.z);
        Debug.Log("Finished.");
        
        retracingPath = false;
    }
    
    void UpdateNavigation()
    {
        surface.BuildNavMesh();
    }


    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY); // positive

        return 14 * distX + 10 * (distY - distX); // negative

    }

}

// equals SNAKE PATTERN?!@?!?!?!?!?!?!?!?!!?!?!?!?!?!?!?!?!?!?!?!?!?!?!



#region old
/*
   Grid grid;
   public Transform startPosition, endPosition;
   private void Awake()
   {
       grid = GetComponent<Grid>();
   }
   private void Update()
   {
       FindPath(startPosition.position, endPosition.position);
   }
   void FindPath(Vector3 sPos, Vector3 ePos)
   {
       Node startNode = grid.NodeFromWorldPosition(sPos);
       Node endNode = grid.NodeFromWorldPosition(ePos);

       List<Node> openList = new List<Node>();
       HashSet<Node> closedList = new HashSet<Node>();
       openList.Add(startNode);
       while(openList.Count > 0)
       {
           Node currentNode = openList[0];
           for(int i = 1; i < openList.Count; i++)
           {
               if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost)
               {
                   currentNode = openList[i];
               }
           }
           openList.Remove(currentNode);
           closedList.Add(currentNode);
           if(currentNode == endNode)
           {
               GetFinalPath(startNode, endNode);
           }
           foreach (Node neighbor in grid.GetNeighbors(currentNode))
           {
               if(!neighbor.isWall || closedList.Contains(neighbor))
               {
                   continue;
               }
               int moveCost = currentNode.gCost + GetManhattenDistance(currentNode, neighbor);
               if(moveCost < neighbor.gCost || !openList.Contains(neighbor))
               {
                   neighbor.gCost = moveCost;
                   neighbor.hCost = GetManhattenDistance(neighbor, endNode);
                   neighbor.Parent = currentNode;
                   if (!openList.Contains(neighbor))
                   {
                       openList.Add(neighbor);

                   }
               }
           }
       }
   }
   void GetFinalPath(Node s, Node e)
   {
       List<Node> finalPath = new List<Node>();
       Node currentNode = e;
       while(currentNode != s)
       {
           finalPath.Add(currentNode);
           currentNode = currentNode.Parent;
       }
       finalPath.Reverse();
       grid.finalPath = finalPath;
   }
   int GetManhattenDistance(Node x, Node y)
   {
       int ix = Mathf.Abs(x.gridX - y.gridX);
       int iy = Mathf.Abs(x.gridY - y.gridY);

       return ix + iy;
   }
   */
#endregion