using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class TileManager {
    //Used for some calculations.
    public int ModifierPriority;
    public Tile[,] tiles;
    private Map map;

    public TileManager(Map map) {
        this.map = map;
        tiles = new Tile[map.mapChunkSize, map.mapChunkSize];
        for (int x = 0; x < tiles.GetLength(0); x++) {
            for (int y = 0; y < tiles.GetLength(1); y++) {
                tiles[x, y] = new Tile(map, this, new Vector2(x, y), false);
            }
        }
    }

    public Tile[,] GetRandomGridTexel ( Vector2 size ) {
        Vector2 pos = new Vector2(
            Mathf.FloorToInt(Noise.GetRandomNumber(map.noiseData.seed) * ( tiles.GetLength(0) - size.x )),
            Mathf.FloorToInt(Noise.GetRandomNumber(map.noiseData.seed) * ( tiles.GetLength(1) - size.y ))
            );
        return GetGridtexel(pos, size);
    }

    //Position starts in the lower left-hand corner
    public Tile[,] GetGridtexel ( Vector2 position, Vector2 size ) {
        if (position.x < 0 || position.x + size.x >= tiles.GetLength(0) || position.y < 0 || position.y + size.y >= tiles.GetLength(0)) {
            //Invalid position.
            throw new IndexOutOfRangeException("Specified texel coordinates is outside of the valid range." + position + " " + size);
        }
        Tile[,] texel = new Tile[Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y)];
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                texel[x, y] = tiles[Mathf.RoundToInt(position.x) + x, Mathf.RoundToInt(position.y) + y];
            }
        }
        return texel;
    }

    public Tile GetTile(Vector2 position) {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1)) {
            return tiles[x, y];
        }
        return null;
    }
}
