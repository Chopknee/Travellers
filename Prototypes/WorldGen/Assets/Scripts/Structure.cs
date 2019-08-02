using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Structure : UpdateableData {

    public Vector2 texelSize;
    public float radius;
    [Range(0, 1)]
    public float maximumSlope;
    public bool alignToSlope;
    public bool flattenTerrain;
    public int numberToSpawn;
    public GameObject[] structurePrefabs;

    public GameObject GetRandomPrefab(int seed) {
        return structurePrefabs[(int)Noise.GetRandomRange(seed, structurePrefabs.Length)];
    }

#if UNITY_EDITOR

    protected override void OnValidate () {
        texelSize.x = Mathf.RoundToInt(texelSize.x);
        texelSize.y = Mathf.RoundToInt(texelSize.y);
        base.OnValidate();

    }

#endif
}
