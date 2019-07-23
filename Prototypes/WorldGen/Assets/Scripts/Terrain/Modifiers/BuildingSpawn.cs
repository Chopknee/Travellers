using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawn : MonoBehaviour, ITerrainModifier
{
    //This holds the algorithm designed for spawning buildings in the game.
    //Need to break down the variables that go into deciding where to put the structures on the map
    //For now, I am only going to worry about slope requirements.

    public Structure[] Structures;
    public int ModifierPriority;
    private Map map;
    private TileManager tileManager;



    public void GenerateBuildings() {
        map = GetComponent<Map>();
        tileManager = map.tileManager;
        int seed = map.noiseData.seed;
        NoiseData dat = map.noiseData;
        foreach (Structure structure in Structures) {
            int successes = 0;
            while (successes < structure.numberToSpawn) {
                bool doneThing = false;
                float rotation = Noise.GetRandomRange(map.noiseData.seed, 360);//Get a random rotation ready
                float diagonalLength = Mathf.Ceil(structure.texelSize.magnitude);//The length of the diagonal for the texel. Helps prevent issues relating to out of bounds exceptions.
                Vector2 randomCenter = new Vector2(Noise.GetRandomRange(seed, diagonalLength, map.mapChunkSize - 1 - diagonalLength), Noise.GetRandomRange(seed, diagonalLength, map.mapChunkSize - 1 - diagonalLength));
                Tile center = map.tileManager.GetTile(randomCenter);
                Tile[,] buildingTexel = map.GetTilesTransformed(center.gridPosition, -structure.texelSize / 2, structure.texelSize, rotation );
                //Now we can actually check if the slope is too high.
                if (IsLocationValid(buildingTexel, structure)) {

                    doneThing = true;
                    int ind = Mathf.FloorToInt(Noise.GetRandomNumber(map.noiseData.seed) * structure.structurePrefabs.Length);    
                    GameObject building = Instantiate(structure.structurePrefabs[ind], center.position, Quaternion.Euler(new Vector3(0, rotation, 0)), transform);
                    //Flatten out the terrain under this structure
                    Vector2 texelSize = structure.texelSize;

                    float h = center.unscaledHeight;
                    foreach (Tile t in buildingTexel) {
                        t.occupyingObject = building;
                        t.unscaledHeight = h;
                    }

                }

                if (doneThing) {
                    successes++;
                }
            }
        }
    }

    bool IsLocationValid(Tile[,] tiles, Structure candidate) {
        float[,] heights = new float[tiles.GetLength(0), tiles.GetLength(1)];

        for (int x = 0; x < tiles.GetLength(0); x++) {
            for (int y = 0; y < tiles.GetLength(1); y++) {
                heights[x,y] = tiles[x,y].unscaledHeight;
                if (map.tileManager.tiles[x, y].Occupied)
                    return false;
            }
        }

        if (Map.GetAverageGradient(heights).magnitude > candidate.maximumSlope)
            return false;
        return true;
    }

    public int GetPriority () {
        return ModifierPriority;
    }

    public void Modify ( Map map ) {
        GenerateBuildings();
    }
}
