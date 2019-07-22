using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

public class PathfindingGrid {

    public delegate int TileCost(Vector2 currentGridPosition, Vector2 nextGridPosition);
    public delegate bool TilePassable(Vector2 position);
    public delegate void PathRequestComplete(PathResult result);

    public int Width {
        get {
            return width;
        }
    }
    public int Height {
        get {
            return height;
        }
    }

    public int Size {
        get {
            return width * height;
        }
    }

    Node[,] grid;

    int width;
    int height;
    public PathfindingGrid(int width, int height) {
        this.width = width;
        this.height = height;
        grid = new Node[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y] = new Node(null, new Vector2(x, y), 0, 0);
            }
        }
    }

    public Thread RequestPath(Vector2 start, Vector2 end, TileCost tileCost, TilePassable tilePassable, bool useSubCardinals, PathRequestComplete reuqestCompleteFunction) {
        PathRequest pr = new PathRequest(start, end, tileCost, tilePassable, useSubCardinals, grid, reuqestCompleteFunction);
        Thread t = new Thread(pr.FindPath);
        t.Start();
        return t;
    }

    class PathRequest {

        private Vector2 start;
        private Vector2 end;
        private TileCost tileCost;
        private TilePassable tilePassable;
        private bool useSubCardinals;
        private Node[,] grid;
        private PathRequestComplete requestComplete;

        public PathRequest(Vector2 start, Vector2 end, TileCost tileCost, TilePassable tilePassable, bool useSubCardinals, Node[,] grid, PathRequestComplete requestComplete) {
            this.start = start;
            this.end = end;
            this.tileCost = tileCost;
            this.tilePassable = tilePassable;
            this.useSubCardinals = useSubCardinals;
            this.grid = grid;
            this.requestComplete = requestComplete;
        }

        /*
            Returns a path from start to end (if possible)
            Vector2 start - the place to begin searching from
            Vector2 end - the place we're trying to navigate to
            TileCost tileCost - the function that determines any extra movement cost from the current to the next tile
            TilePassable tilePassable - the function that blocks a tile from being moved into
            bool useSubCardinals - if true, then the algorythm will be allowed to make diagonal movments, otherwise it will not
        */
        public void FindPath() {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector2[] waypoints = new Vector2[0];
            bool pathSuccess = false;
            Node startNode = GetNode(start);
            Node endNode = GetNode(end);
            startNode.parent = startNode;

            if (tilePassable(startNode.position) && tilePassable(endNode.position)) {
                Heap<Node> openSet = new Heap<Node>(grid.GetLength(0) * grid.GetLength(1));
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);
                while (openSet.Count > 0) {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == endNode) {
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbor in GetNeighbors(currentNode, useSubCardinals)) {
                        if (!tilePassable(neighbor.position) || closedSet.Contains(neighbor)) {
                            continue;
                        }

                        int newMoveCostToNeighbor = currentNode.g + GetDistance(currentNode, neighbor) + tileCost(currentNode.position, neighbor.position);
                        if (newMoveCostToNeighbor < neighbor.g || !openSet.Contains(neighbor)) {
                            neighbor.g = newMoveCostToNeighbor;
                            neighbor.h = GetDistance(neighbor, endNode);
                            neighbor.parent = currentNode;
                            if (!openSet.Contains(neighbor)) {
                                openSet.Add(neighbor);
                            } else {
                                openSet.UpdateItem(neighbor);
                            }
                        }
                    }
                    
                }
            }

            if (pathSuccess) {
                waypoints = RetracePath(startNode, endNode);
            }
            UnityEngine.Debug.Log("Path found in " + (sw.ElapsedMilliseconds / 1000f) + " seconds. Total path length is " + waypoints.Length + " nodes.");
            sw.Stop();
            
            requestComplete(new PathResult(waypoints, pathSuccess));
            //return new PathResult(waypoints, pathSuccess);
        }

        Vector2[] RetracePath(Node start, Node end) {
            List<Vector2> path = new List<Vector2>();
            Node currentNode = end;

            while (currentNode != start) {
                path.Add(currentNode.position);
                currentNode = currentNode.parent;
            }
            return path.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB) {
            int distX = (int)Mathf.Abs(nodeA.position.x- nodeB.position.x);
            int distY = (int)Mathf.Abs(nodeA.position.y- nodeB.position.y);
            if (distX > distY) {
                return 14*distY + 10*(distX-distY);
            }
            return 14*distX + 10*(distY-distX);
        }


        public Node GetNode(Vector2 position) {
            if ((int)position.x >= 0 && (int) position.x < grid.GetLength(0) && (int)position.y >= 0 && (int) position.y < grid.GetLength(1)) {
                return grid[(int)position.x, (int)position.y];
            }
            return null;
        }

        public List<Node> GetNeighbors(Node node, bool useSubCardinals) {
            List<Node> neighbors = new List<Node>();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    //A very clunky if statement but whatever....
                    if ( useSubCardinals && x == 0 && y == 0 //If we want to use all 8 possible directions
                    || !useSubCardinals && x == -1 && y == -1 || !useSubCardinals && x == 1 && y == -1//If we want to only use the 4 direct neighbors
                    || !useSubCardinals && x == 1 && y == 1 || !useSubCardinals && x == 1 && y == -1) {
                        continue;
                    }
                    int checkX = (int)node.position.x + x;
                    int checkY = (int)node.position.y + y;
                    if (checkX >= 0 && checkX < grid.GetLength(0) && checkY >= 0 && checkY < grid.GetLength(1)) {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }


            return neighbors;
        }
    }
}

public class PathResult {
    private Vector2[] myPath;
    bool myResult;
    public Vector2[] path {
        get {
            return myPath;
        }
    }
    public bool result {
        get {
            return myResult;
        }
    }
    public PathResult(Vector2[] path, bool result) {
        myPath = path;
        myResult = result;
    }
}