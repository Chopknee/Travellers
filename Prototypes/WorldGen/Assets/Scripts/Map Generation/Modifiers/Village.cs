using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour, ITerrainModifier {

    public Structure[] structures;
    private Map mapGen;
    private TileManager tileManager;
    public float structureRadius;
    private List<GameObject> instantiatedStructures;

    public int ModifierPriority = 0;

    public int maximumSpawnAttempts = 20;

    public int GetPriority () {
        return ModifierPriority;
    }

    public void Modify ( Map map ) {
        mapGen = map;
        tileManager = mapGen.tileManager;

        //Using a poisson style distrobution, spawn in a village.
        //  Generate a list of buildings that need to be spawned in
        List<Structure> structuresToSpawn = new List<Structure>();
        foreach (Structure s in structures) {
            for (int i = 0; i < s.numberToSpawn; i ++) {
                //Selects a random building prefab to add into this list. (Each building will be guarenteed to be spawned)
                structuresToSpawn.Add(s);
            }
        }

        int attempts = 0;
        //I want this to be independent of other things, so create a new seed.
        int seed = map.noiseData.seed + 1;
        Vector2 sampleRegionSize = new Vector2(map.mapChunkSize, map.mapChunkSize);
        while (structuresToSpawn.Count > 0 || attempts < maximumSpawnAttempts) {
            //Spawning in buildings.

            //This is where the poisson disk should be set up.
            float cellSize = structureRadius / Mathf.Sqrt(2);
            int[,] poissonGrid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> spawnPoints = new List<Vector2>();

            spawnPoints.Add(new Vector2(Noise.GetRandomRange(seed, sampleRegionSize.x), Noise.GetRandomRange(seed, sampleRegionSize.y)));
            while (spawnPoints.Count > 0 && structuresToSpawn.Count > 0) {
                int spawnIndex = Noise.GetRandomRange(seed, 0, spawnPoints.Count);

                Vector2 spawnCenter = spawnPoints[spawnIndex];
                bool candidateAccepted = false;
                for (int i = 0; i < 30; i++) {
                    float angle = Noise.GetRandomNumber(seed) * PI2;
                    Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                    Vector2 candidate = spawnCenter + dir * Noise.GetRandomRange(seed, structureRadius, 2 * structureRadius);

                    //Get a random structure to spawn from the list.
                    int structureIndex = Mathf.FloorToInt(Noise.GetRandomRange(map.noiseData.seed, structuresToSpawn.Count));
                    //Get a random rotation
                    float rotation = Noise.GetRandomRange(map.noiseData.seed, 360);

                    if (IsValid(candidate, sampleRegionSize, cellSize, structureRadius, points, poissonGrid) && IsSlopeGood(candidate, structuresToSpawn[structureIndex], map, rotation)) {
                        //Extra conditions include if the gradient/slope of the location is able to support the structure
                        //Also need to figure out how the mapping converts
                        points.Add(candidate);
                        spawnPoints.Add(candidate);
                        poissonGrid[(int) ( candidate.x / cellSize ), (int) ( candidate.y / cellSize )] = points.Count;
                        candidateAccepted = true;
                        //Create the structure at the proper position
                        GameObject building = Instantiate(structuresToSpawn[structureIndex].GetRandomPrefab(map.noiseData.seed), map.TerrainCoordToRealWorld(candidate), Quaternion.Euler(new Vector3(0, rotation, 0)), transform);
                        //Flatten out the terrain under this structure
                        Vector2 texelSize = structuresToSpawn[structureIndex].texelSize;

                        float h = tileManager.tiles[Mathf.RoundToInt(candidate.x), Mathf.RoundToInt(candidate.y)].unscaledHeight;
                        Tile[,] tilesUnderStructure = map.GetTilesTransformed(candidate, -texelSize / 2, texelSize, rotation);

                        foreach (Tile t in tilesUnderStructure) {
                            t.occupyingObject = building;
                            t.unscaledHeight = h;
                        }
                        
                        //Remove the structure from the spawn list.
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

    bool IsValid( Vector2 candidate, Vector2 sampleRegionSize, float cellsize, float radius, List<Vector2> points, int[,] grid) {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y ) {
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

    bool IsSlopeGood(Vector2 candidateCenter, Structure candidateSpawn, Map map, float angle) {
        Vector2 texelSize = candidateSpawn.texelSize;
        float[,] texel = map.GetHeightsTransformed(candidateCenter, -texelSize / 2, texelSize, angle);
        return Map.GetAverageGradient(texel).magnitude < candidateSpawn.maximumSlope;
    }

    float[,] GenerateHeightArray(float h, Vector2 size) {
        float[,] heights = new float[Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y)];
        for (int x = 0; x < Mathf.RoundToInt(size.x); x++) {
            for (int y = 0; y < Mathf.RoundToInt(size.y); y++) {
                heights[x, y] = h;
            }
        }
        return heights;
    }

}
