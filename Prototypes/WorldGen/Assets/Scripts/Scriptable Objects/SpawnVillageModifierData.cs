using BaD.Modules.Networking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Terrain.Modifiers {
    [CreateAssetMenu()]
    public class SpawnVillageModifierData: AModifierData {
        public Structure villageCenter;
        public Structure[] structures;
        public float structureRadius;
        private List<GameObject> instantiatedStructures;

        public int maximumSpawnAttempts = 20;

        private Vector2 sampleRegionSize;
        private int seed;
        private float cellSize;
        List<Vector2> points;
        List<Vector2> spawnPoints;
        int[,] poissonGrid;
        public override void Execute ( Map map ) {

            TileManager tileManager = map.tileManager;
            sampleRegionSize = new Vector2(map.mapChunkSize, map.mapChunkSize);
            seed = map.noiseData.seed + 1;
            cellSize = structureRadius / Mathf.Sqrt(2);

            poissonGrid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
            points = new List<Vector2>();
            spawnPoints = new List<Vector2>();

            bool spawned = false;
            GameObject centralVillageGameObject;
            Vector2 villageCenterPosition = Vector2.zero;
            int attempts = 0;

            while (spawned != true) {
                villageCenterPosition = new Vector2(Noise.GetRandomRange(seed, sampleRegionSize.x), Noise.GetRandomRange(seed, sampleRegionSize.y));
                if (( centralVillageGameObject = TrySpawn(villageCenterPosition, this.villageCenter, map)) != null) {
                    spawned = true;
                }
                attempts++;
                if (attempts > maximumSpawnAttempts) {
                    Debug.LogError("Could not spawn a village, the maximum number of spawn attempts has been exceeded.");
                    return;
                }
            }

            //  Generate a list of buildings that need to be spawned in
            List<Structure> structuresToSpawn = new List<Structure>();
            foreach (Structure s in structures) {
                for (int i = 0; i < s.numberToSpawn; i++) {
                    structuresToSpawn.Add(s);
                }
            }

            attempts = 0;

            while (structuresToSpawn.Count > 0 || attempts < maximumSpawnAttempts) {

                spawnPoints.Add(villageCenterPosition);
                while (spawnPoints.Count > 0 && structuresToSpawn.Count > 0) {
                    int spawnIndex = Noise.GetRandomRange(seed, 0, spawnPoints.Count);

                    Vector2 spawnCenter = spawnPoints[spawnIndex];
                    bool candidateAccepted = false;
                    for (int i = 0; i < 30; i++) {
                        float angle = Noise.GetRandomNumber(seed) * PI2;
                        Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                        Vector2 candidate = spawnCenter + dir * Noise.GetRandomRange(seed, structureRadius, 2 * structureRadius);

                        //Get a random structure to spawn from the list.
                        int structureIndex = Mathf.FloorToInt(Noise.GetRandomRange(seed, structuresToSpawn.Count));
                        GameObject go;
                        if ((go = TrySpawn(candidate, structuresToSpawn[structureIndex], map)) != null) {
                            points.Add(candidate);
                            spawnPoints.Add(candidate);
                            poissonGrid[(int) ( candidate.x / cellSize ), (int) ( candidate.y / cellSize )] = points.Count;
                            candidateAccepted = true;
                            structuresToSpawn.RemoveAt(structureIndex);
                            break;
                        }
                    }
                    if (!candidateAccepted) {
                        spawnPoints.RemoveAt(spawnIndex);
                    }
                }
                attempts++;
            }
        }

        public GameObject TrySpawn(Vector2 position, Structure structure, Map map) {
            //Random rotation of the structure
            float rotation = Noise.GetRandomRange(map.noiseData.seed, 360);
            //An array filled with all tiles underneath this structure
            Tile[,] tilesUnderStructure = map.GetTilesFromRadius(position, structure.radius);
            //Checking if the position selected is a valid place to spawn this structure
            if (IsValid(position, sampleRegionSize, cellSize, structureRadius, points, poissonGrid) && IsSlopeGood(tilesUnderStructure, map, structure)) {
                //Instantiate a random prefab of this structure
                GameObject building = Instantiate(structure.GetRandomPrefab(seed));                
                building.transform.SetParent(map.transform);
                Vector2 exactoPositiono = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
                building.transform.position = map.TerrainCoordToRealWorld(exactoPositiono);
                building.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
                //Give the structure a link to it's spawn information
                building.AddComponent<StructureDataLink>().structureData = structure;

                //Get the height of the tile directly under this structure
                float h = map.tileManager.tiles[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)].unscaledHeight;

                //Go through each tile under this structure and flatten it out.
                foreach (Tile t in tilesUnderStructure) {
                    if (t == null)
                        continue;
                    //Also make sure the tile cannot be travelled in
                    t.Blocked = true;
                    //Make the space under the structure into a path
                    map.pathMap[(int) t.gridPosition.x, (int) t.gridPosition.y] = 1;
                    t.unscaledHeight = h;
                }

                //Remove the structure from the spawn list.
                return building;
            }
            return null;
        }

        private readonly float PI2 = Mathf.PI * 2;

        public int TotalStructureCount {
            get {
                int count = 0;
                foreach (Structure s in structures) {
                    count += s.numberToSpawn;
                }
                return count;
            }
        }

        //For the poisson distribution
        bool IsValid ( Vector2 candidate, Vector2 sampleRegionSize, float cellsize, float radius, List<Vector2> points, int[,] grid ) {
            if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) {
                int cellX = (int) ( candidate.x / cellsize );
                int cellY = (int) ( candidate.y / cellsize );
                int searchStartX = Mathf.Max(0, cellX - 2);
                int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
                int searchStartY = Mathf.Max(0, cellY - 2);
                int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

                for (int x = 0; x < searchEndX; x++) {
                    for (int y = 0; y < searchEndY; y++) {
                        int pointIndex = grid[x, y] - 1;
                        if (pointIndex != -1) {
                            float sqrDst = ( candidate - points[pointIndex] ).sqrMagnitude;
                            if (sqrDst < radius * radius) {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        bool IsSlopeGood ( Tile[,] tiles, Map map, Structure candidate ) {
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
    }
}
