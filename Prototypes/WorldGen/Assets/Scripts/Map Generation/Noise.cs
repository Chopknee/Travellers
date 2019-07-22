using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode) {
        float[,] noiseMap = new float[mapWidth, mapHeight];


        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = GetPRNG(seed).Next(-100000, 100000) + offset.x;
            float offsetY = GetPRNG(seed).Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                } else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                if (normalizeMode == NormalizeMode.Local) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                } else {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }

    public static float GetRandomNumber(int seed) {
        return (float)GetPRNG(seed).NextDouble();
    }


    public static float GetRandomRange(int seed, float min, float max) {
        float range = max - min;
        float num = (float)( GetPRNG(seed).NextDouble() * range ) + min;
        return num;//Turn this into a range
    }

    public static float GetRandomRange ( int seed, float max ) {
        return GetRandomRange(seed, 0, max);
    }

    public static int GetRandomRange(int seed, int min, int max) {
        return Mathf.FloorToInt(GetRandomRange(seed, (float)min, (float)max));
    }

    private static Dictionary<int, System.Random> randoms;
    private static Dictionary<string, Vector2[]> octaveOffsetsLookup;

    private static System.Random GetPRNG(int seed) {
        if (randoms == null) {
            randoms = new Dictionary<int, System.Random>();
        }
        if (!randoms.ContainsKey(seed)) {
            randoms.Add(seed, new System.Random(seed));
        }
        return randoms[seed];
    }

    public static void Reset(int seed) {
        randoms.Remove(seed);
        randoms.Add(seed, new System.Random(seed));
    }

    public static float GetNoiseValue ( int x, int y, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset ) {


        
        float amplitude = 1;
        float frequency = 1;
        //Cacheing the octave offsets for successive lookups.
        Vector2[] octaveOffsets;
        string Key = octaves + "" + offset.x + "" + offset.y;
        if (octaveOffsetsLookup == null) {
            octaveOffsetsLookup = new Dictionary<string, Vector2[]>();
        }

        if (octaveOffsetsLookup.ContainsKey(Key)) {
            octaveOffsets = octaveOffsetsLookup[Key];
        } else {
            octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++) {
                float offsetX = GetPRNG(seed).Next(-100000, 100000) + offset.x;
                float offsetY = GetPRNG(seed).Next(-100000, 100000) - offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                amplitude *= persistance;
                octaveOffsetsLookup.Add(Key, octaveOffsets);
            }
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        amplitude = 1;
        frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++) {
            float sampleX = ( x - octaveOffsets[i].x ) / scale * frequency;
            float sampleY = ( y - octaveOffsets[i].y ) / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        return noiseHeight;
    }

}