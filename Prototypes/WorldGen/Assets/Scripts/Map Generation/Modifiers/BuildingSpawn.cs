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
    private Map mapGen;
    private TileManager tileManager;



    public void GenerateBuildings() {
        mapGen = GetComponent<Map>();
        tileManager = mapGen.tileManager;
        int seed = mapGen.noiseData.seed;
        NoiseData dat = mapGen.noiseData;
        foreach (Structure structure in Structures) {
            int successes = 0;
            while (successes < structure.numberToSpawn) {
                bool doneThing = false;
                Tile[,] tiles = tileManager.GetRandomGridTexel(structure.texelSize);
                Tile center = tiles[tiles.GetLength(0) / 2, tiles.GetLength(1) / 2];
                //Now we can actually check if the slope is too high.
                Vector2 gradient = Map.GetAverageGradient(Tile.ToTexel(tiles, false));
                if (gradient.magnitude < structure.maximumSlope) {
                    int ind = Mathf.FloorToInt(Noise.GetRandomNumber(mapGen.noiseData.seed) * structure.structurePrefabs.Length);
                    GameObject go = Instantiate(structure.structurePrefabs[ind]);
                    go.transform.SetParent(transform);
                    go.transform.position = new Vector3(center.position.x, center.scaledHeight, center.position.z);
                    doneThing = true;
                }

                if (doneThing) {
                    successes++;
                }
            }
        }
    }

    public int GetPriority () {
        return ModifierPriority;
    }

    public void Modify ( Map map ) {
        GenerateBuildings();
    }
}
