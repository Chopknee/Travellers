using BaD.Modules.Terrain.Modifiers;
using System;
using UnityEngine;

namespace BaD.Modules.Terrain {
    [CreateAssetMenu]
    public class BuildingSpawn : AModifierData {
        //This holds the algorithm designed for spawning buildings in the game.
        //Need to break down the variables that go into deciding where to put the structures on the map
        //For now, I am only going to worry about slope requirements.

        public Structure[] Structures;
        public int ModifierPriority;
        private Map map;
        private TileManager tileManager;



        public override void Execute ( Map map ) {
            tileManager = map.tileManager;
            int seed = map.noiseData.seed;
            NoiseData dat = map.noiseData;
            foreach (Structure structure in Structures) {
                int successes = 0;
                while (successes < structure.numberToSpawn) {
                    bool doneThing = false;
                    float rotation = Noise.GetRandomRange(map.noiseData.seed, 360);//Get a random rotation ready
                    Vector2 randomCenter = new Vector2(Noise.GetRandomRange(seed, structure.radius, map.mapChunkSize - 1 - structure.radius), Noise.GetRandomRange(seed, structure.radius, map.mapChunkSize - 1 - structure.radius));
                    Tile center = map.tileManager.GetTile(randomCenter);

                    Tile[,] buildingTexel = map.GetTilesFromRadius(center.gridPosition, structure.radius);
                    //Now we can actually check if the slope is too high.
                    if (IsLocationValid(buildingTexel, map, structure)) {

                        doneThing = true;
                        int ind = Mathf.FloorToInt(Noise.GetRandomNumber(map.noiseData.seed) * structure.structurePrefabs.Length);
                        GameObject building = Instantiate(structure.structurePrefabs[ind], center.position, Quaternion.Euler(new Vector3(0, rotation, 0)), map.transform);
                        //Flatten out the terrain under this structure

                        float h = center.unscaledHeight;
                        foreach (Tile t in buildingTexel) {
                            if (t == null)
                                continue;
                            t.Blocked = true;
                            t.unscaledHeight = h;
                            map.pathMap[(int) t.gridPosition.x, (int) t.gridPosition.y] = 1;
                        }
                    }

                    if (doneThing) {
                        successes++;
                    }
                }
            }
        }

        bool IsLocationValid ( Tile[,] tiles, Map map, Structure candidate ) {
            float accum = 0;
            float tCount = 0;
            //Calculates the average slop of the group of tiles.
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    if (tiles[x, y] != null) {
                        accum += tiles[x, y].unscaledHeight;
                        tCount++;
                        if (tiles[x, y].Blocked)
                            return false;
                    }
                }
            }

            accum /= tCount;//The average height of the entire thing.
                            //Fills the texel with the values from tile, setting any null values to the average height of the whole area.
            float[,] texel = new float[tiles.GetLength(0), tiles.GetLength(1)];
            for (int x = 0; x < tiles.GetLength(0); x++) {
                for (int y = 0; y < tiles.GetLength(1); y++) {
                    if (tiles[x, y] != null) {
                        texel[x, y] = tiles[x, y].unscaledHeight;
                    } else {
                        texel[x, y] = accum;
                    }
                }
            }

            return Map.GetAverageGradient(texel).magnitude < candidate.maximumSlope;
        }

        public int GetPriority () {
            return ModifierPriority;
        }

        // bool IsLocationValid(Tile[,] tiles, Structure candidate) {
        //     float[,] heights = new float[tiles.GetLength(0), tiles.GetLength(1)];

        //     for (int x = 0; x < tiles.GetLength(0); x++) {
        //         for (int y = 0; y < tiles.GetLength(1); y++) {
        //             heights[x,y] = tiles[x,y].unscaledHeight;
        //             if (map.tileManager.tiles[x, y].Occupied)
        //                 return false;
        //         }
        //     }

        //     if (Map.GetAverageGradient(heights).magnitude > candidate.maximumSlope)
        //         return false;
        //     return true;
        // }
    }
}