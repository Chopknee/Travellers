using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    private Map map;
    private TileManager tileManager;
    public GameObject occupyingObject;
    private Vector2 terrainCoordinate;
    //public 
    public Tile ( Map map, TileManager grid, Vector2 terrainCoordinate ) {
        this.map = map;
        this.tileManager = grid;
        this.terrainCoordinate = terrainCoordinate;
    }

    public float scaledHeight {
        get {
            if (map!= null) {
                return map.GetScaledHeight(terrainCoordinate); 
            }
            return -1;
        }
    }

    public float unscaledHeight {
        get {
            if (map != null) {
                return map.GetHeight(terrainCoordinate);
            }
            return -1;
        }
        set {
            if (map != null) {
                map.heights[(int) terrainCoordinate.x, (int) terrainCoordinate.y] = value;
            }
        }
    }

    public bool Occupied {
        get {
            return occupyingObject != null;
        }
    }

    public Vector3 position {
        get {
            if (map != null) {
                return map.TerrainCoordToRealWorld(terrainCoordinate);
            }
            return Vector3.zero;
        }
    }

    public Vector2 gridPosition {
        get {
            return new Vector2(terrainCoordinate.x, terrainCoordinate.y);
        }
    }

    public Tile GetRelative ( Vector2 offset ) {
        Vector2 coord = terrainCoordinate + offset;
        if (coord.x < tileManager.tiles.GetLength(0) && coord.x >= 0 && coord.y < tileManager.tiles.GetLength(1) && coord.y >= 0) {
            return tileManager.tiles[(int) coord.x, (int) coord.y];
        }
        return null;
    }

    public float heightCost {
        get {
            return unscaledHeight;
        }
    }

    public bool CanWalkHere() {
        return !Occupied;
    }

    public static float[,] ToTexel(Tile[,] tiles, bool scaledHeight) {
        float[,] tex = new float[tiles.GetLength(0), tiles.GetLength(1)];
        for (int x = 0; x < tiles.GetLength(0); x++) {
            for (int y = 0; y < tiles.GetLength(1); y++) {
                tex[x, y] = (scaledHeight)? tiles[x, y].scaledHeight : tiles[x,y].unscaledHeight;
            }
        }
        return tex;
    }
}
