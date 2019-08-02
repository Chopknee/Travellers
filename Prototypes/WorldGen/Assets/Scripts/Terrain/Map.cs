using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour {
    [Range(0, MeshGenerator.numSupportedChunkSizes - 1)]
    public int TerrainSize;
    [Range(0, MeshGenerator.numSupportedLODs - 1)]
    public int LevelOfDetail;

    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;
    public Material terrainMaterial;
    public int elevationChangePenalty;
    //
    public float[,] heights;

    public float[,] pathMap;

    public GameObject terrainMeshObject;
    //private MapDisplay mapDisplay;
    private MeshFilter terrainMeshFilter;
    private MeshRenderer terrainMeshRenderer;

    public bool UpdateInEditor = false;
    public TileManager tileManager;
    public PathfindingGrid pathfinder;

    public bool BlurPath = true;
    [Range(0, 15)]
    public int BlurRadius = 7;

    public int mapChunkSize {
        get {
            return MeshGenerator.supportedChunkSizes[TerrainSize] - 1;
        }
    }

    public void Awake() {
        tileManager = new TileManager(this);
        pathfinder = new PathfindingGrid(mapChunkSize, mapChunkSize);
    }

    public void GenerateTerrain() {
        pathMap = new float[mapChunkSize+2, mapChunkSize+2];
        heights = GenerateMapData(Vector2.zero);
        //Get all terrain modifiers currently attatched
        ITerrainModifier[] terrainModifiers = GetComponents<ITerrainModifier>();
        //Order them by priority
        IEnumerable<ITerrainModifier> modifiers = terrainModifiers.OrderBy(terrain => terrain.GetPriority());
        //Execute the modifiers
        foreach (ITerrainModifier tm in modifiers) {
            tm.Modify(this);
        }
        terrainMaterial.SetFloat("Vector1_43C4DCE9",terrainData.maxHeight);

        if (BlurPath) {
            TextureGenerator.BlurMap(pathMap, BlurRadius);//Will it work???
        }

        terrainMaterial.SetTexture("Texture2D_1F7D3F7B", TextureGenerator.TextureFromHeightMap(pathMap));
    }

    public void CreateMesh() {
        terrainMeshRenderer.material = terrainMaterial;
        terrainMeshFilter.sharedMesh = MeshGenerator.GenerateTerrainMesh(heights, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, LevelOfDetail, false).CreateMesh();
    }

    float[,] GenerateMapData ( Vector2 center ) {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);
        return noiseMap;
    }

    public float[,] GetTerrainChunk ( Vector2 northWestPoint, Vector2 size ) {
        if (northWestPoint.x < 0 || northWestPoint.x + size.x - 1 > heights.GetLength(0) || northWestPoint.y < 0 || northWestPoint.y + size.y - 1 > heights.GetLength(1)) {
            float[,] chunk = new float[Mathf.FloorToInt(size.x), Mathf.FloorToInt(size.y)];
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    chunk[x, y] = heights[Mathf.FloorToInt(northWestPoint.x) + x, Mathf.FloorToInt(northWestPoint.y) + y];
                }
            }
            return null;
        }
        throw new IndexOutOfRangeException("The requested terrain chunk is out of bounds.");
    }


    private void CreateTerrainMeshObject () {
        if (terrainMeshObject == null) {
            GameObject go = new GameObject("Game Terrain");
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            terrainMeshObject = go;
            Vector3 scl = new Vector3(1, 1, -1);
            terrainMeshObject.transform.localScale = scl * terrainData.uniformScale;
            terrainMeshObject.transform.SetParent(transform);
        }
        terrainMeshFilter = terrainMeshObject.GetComponent<MeshFilter>();
        terrainMeshRenderer = terrainMeshObject.GetComponent<MeshRenderer>();
    }

    public void Generate() {
        CreateTerrainMeshObject();
        GenerateTerrain();
        CreateMesh();
    }

    public void GenerateInEditor() {

        if (terrainMeshObject != null) {
            if (terrainMeshFilter == null) {
                terrainMeshFilter = terrainMeshObject.GetComponent<MeshFilter>();
                terrainMeshRenderer = terrainMeshObject.GetComponent<MeshRenderer>();
            }
            terrainMeshObject.transform.localScale = Vector3.one * terrainData.uniformScale;
            //CreateTerrainMeshObject();
            GenerateTerrain();
            CreateMesh();
            Noise.Reset(noiseData.seed);
        }
    }

    void OnValuesUpdated () {
        if (!Application.isPlaying) {
            GenerateInEditor();
        }
    }

    void OnTextureValuesUpdated () {
        textureData.UpdatMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
        textureData.ApplyToMaterial(terrainMaterial);
    }

    private void OnValidate () {
        if (terrainData != null) {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null) {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null) {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }

    public float heightScale {
        get {
            return terrainData.meshHeightMultiplier * terrainData.uniformScale;
        }
    }

    public float GetHeight(Vector2 pos) {
        return heights[(int) pos.x, (int) pos.y];
    }

    public void SetHeight(Vector2 pos, float value) {
        heights[(int) pos.x, (int) pos.y] = value;
    }

    public float GetScaledHeight(Vector2 pos) {
        float height = heights[(int) pos.x, (int) pos.y];
        height = terrainData.meshHeightCurve.Evaluate(height) * height * heightScale;
        return height;
    }

    public void SetScaledHeight ( Vector2 pos, float value ) {
        heights[(int) pos.x, (int) pos.y] = value;
    }

    public Tile[,] GetTilesFromRadius(Vector2 center, float radius) {
        float radSquared = radius * radius;
        Tile[,] tiles = new Tile[(int)radius*2+1, (int)radius*2+1];//There is always a center point
        for (int x = -(int)radius; x < (int)radius+1; x ++) {
            for (int y = -(int)radius; y < (int)radius+1; y++) {
                Tile t = tileManager.GetTile(new Vector2(center.x + x, center.y + y));
                if (t != null) {//If in range of the array.
                    Vector2 pos = center + new Vector2(x, y);
                    pos = pos - center;
                    if (pos.sqrMagnitude <= radSquared)
                        tiles[x+(int)radius, y+(int)radius] = t;
                }
            }
        }
        return tiles;
    }

    public Tile[,] GetTilesTransformed(Vector2 position, Vector2 center, Vector2 size, float angle) {
        //The rotation matrix
        float[,] rotationMatrix = Get2DRotationMatrix(angle);
        Tile[,] rotatedHeights = new Tile[(int) size.x, (int) size.y];
        for (int x = 0; x < size.x; x ++) {
            for (int y = 0; y < size.y; y++) {
                //Do the matrix math here. It is entirely possible to get invalid coordinates from this
                Vector2 rotatedCoord = position + GetRotatedCoords(new Vector2(x, y)+center, rotationMatrix);
                if (rotatedCoord.x > -1 && rotatedCoord.x < heights.GetLength(0) && rotatedCoord.y > -1 && rotatedCoord.y < heights.GetLength(1)) {
                    rotatedHeights[x, y] = tileManager.GetTile(rotatedCoord);
                } else {
                    //For any values outside the edge of the map
                    rotatedHeights[x, y] = null;
                }
            }
        }
        return rotatedHeights;
    }

    public float[,] GetHeightsTransformed(Vector2 position, Vector2 center, Vector2 size, float angle) {
        //The rotation matrix
        float[,] rotationMatrix = Get2DRotationMatrix(angle);
        float[,] rotatedHeights = new float[(int) size.x, (int) size.y];
        for (int x = 0; x < size.x; x ++) {
            for (int y = 0; y < size.y; y++) {
                //Do the matrix math here. It is entirely possible to get invalid coordinates from this
                Vector2 rotatedCoord = position + GetRotatedCoords(new Vector2(x, y)+center, rotationMatrix);
                if (rotatedCoord.x > -1 && rotatedCoord.x < heights.GetLength(0) && rotatedCoord.y > -1 && rotatedCoord.y < heights.GetLength(1)) {
                    rotatedHeights[x, y] = heights[(int)rotatedCoord.x, (int)rotatedCoord.y];
                } else {
                    //For any values outside the edge of the map
                    rotatedHeights[x, y] = -1;
                }
            }
        }
        return rotatedHeights;
    }

    /**
     * Sets up a new rotation matrix that can be used for rotating a point.
     */
    private float[,] Get2DRotationMatrix(float angle) {
        float rad = angle * Mathf.Deg2Rad - (Mathf.PI / 4);
        float[,] mat = new float[2, 2];
        mat[0, 0] = Mathf.Cos(rad); mat[0, 1] = -Mathf.Sin(rad);
        mat[1, 0] = Mathf.Sin(rad); mat[1, 1] = Mathf.Cos(rad);
        return mat;
    }

    private Vector2 GetRotatedCoords(Vector2 pos, float[,] rotation) {
        return new Vector2(rotation[0, 0] * pos.x + rotation[0, 1] * pos.y, rotation[1, 0] * pos.x + rotation[1, 1] * pos.y);
    }

    public Vector3 TerrainCoordToRealWorld(Vector2 pos) {
        int halfWidth = Mathf.RoundToInt(mapChunkSize * 0.5f);
        int halfHeight = Mathf.RoundToInt(mapChunkSize * 0.5f);
        return new Vector3((-halfWidth + pos.x) * terrainData.uniformScale, GetScaledHeight(pos), (-halfHeight + pos.y) * terrainData.uniformScale);
    }

    public Vector2 RealWorldToTerrainCoord(Vector3 pos) {
        int halfWidth = Mathf.RoundToInt(mapChunkSize * 0.5f);
        int halfHeight = Mathf.RoundToInt(mapChunkSize * 0.5f);
        return new Vector2(pos.x / terrainData.uniformScale + halfWidth, (pos.z / terrainData.uniformScale) + halfHeight);
    }

    public int GetPathfindingTileCost(Vector2 currentPosition, Vector2 nextPosition) {
        return (int)Mathf.Max(0, ((GetHeight(nextPosition) - GetHeight(currentPosition)) * elevationChangePenalty));
    }

    public bool TileIsPassable(Vector2 position) {
        return tileManager.GetTile(position).CanWalkHere();//For now, all tiles will be marked as passable.
    }
    //Performs a 'raycast' that tries to find a point on the terrain.
    public TerrainClick TerrayCast(float castStepSize, Vector3 forward, Vector3 start, float max) {
        TerrainClick res = new TerrainClick();
        res.success = false;
        float lastY = 0;
        for (float i = 0; i < max; i+=castStepSize) {
            Vector3 newPos = start + (forward * i);
            Tile t = tileManager.GetTile(RealWorldToTerrainCoord(newPos));
            if (t != null) {
                if (i == 0) 
                    lastY = newPos.y;

                if (Mathf.Abs(t.position.y - lastY) > Mathf.Abs(t.position.y - newPos.y)) {
                    lastY = newPos.y;
                    res.success = true;
                    res.terrainPoint = t;
                }

                if (newPos.y <= t.position.y) {
                    break;//No sense in continuing on if the ray goes through the terrain
                }
            }
        }
        return res;
    }

    public static Vector2 GetAverageGradient(float[,] map) {
        float rowSlope = GetAverageSlope(GetRowSums(map));
        float columnSlope = GetAverageSlope(GetColumnSums(map));
        return new Vector2(rowSlope, columnSlope);
    }

    public static Vector2 GetAverageGradient(Tile[,] map) {
        float[,] heights = new float[map.GetLength(0), map.GetLength(1)];
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                heights[x,y] = map[x,y].unscaledHeight;
            }
        }
        return GetAverageGradient(heights);
    }

    private static float[] GetRowSums(float[,] map) {
        //Rows
        float[] sums = new float[map.GetLength(0)];
        for (int x = 0; x < map.GetLength(0); x++) {
            float num = 0;
            for (int y = 0; y < map.GetLength(1); y++) {
                num += map[x, y];
            }
            sums[x] = num;
        }
        return sums;
    }

    private static float[] GetColumnSums ( float[,] map ) {
        //Rows
        float[] sums = new float[map.GetLength(1)];
        for (int y = 0; y < map.GetLength(1); y++) {
            float num = 0;
            for (int x = 0; x < map.GetLength(0); x++) {
                num += map[x, y];
            }
            sums[y] = num;
        }
        return sums;
    }

    private static float GetAverageSlope (float[] points) {
        float slope = 0;
        for (int i = 0; i < points.Length; i++) {
            if (i < points.Length - 1) {
                
                slope +=  (points[i] - points[i + 1]) / ( i - (i + 1.0f) ) ;
            }
        }
        return slope / points.Length;
    }
}

public struct TerrainClick {
    public bool success;
    public Tile terrainPoint;
}

    // public void SetHeightsTransformed(Vector2 position, Vector2 center, Vector2 size, float angle, float[,] values) {
    //     float[,] rotationMatrix = Get2DRotationMatrix(angle);
    //     //float[,] reverseRotationMatrix = Get2DRotationMatrix(-angle);
    //     for (int x = 0; x < values.GetLength(0); x++) {
    //         for (int y = 0; y < values.GetLength(1); y++) {
    //             Vector2 rotatedCoord = position + GetRotatedCoords(new Vector2(x, y)+center, rotationMatrix);
    //             if (rotatedCoord.x >= 0 && rotatedCoord.x < heights.GetLength(0) && rotatedCoord.y >= 0 && rotatedCoord.y < heights.GetLength(1)) {
    //                 //Debug.Log(rotatedCoord.x + " " + rotatedCoord.y);
    //                 heights[Mathf.FloorToInt(rotatedCoord.x), Mathf.FloorToInt(rotatedCoord.y)] = values[x, y];
    //             }//Ignore coords outside the valid range?
    //             else {
    //                 Debug.Log("Invalid index.");
    //             }
    //         }
    //     }
    // }
