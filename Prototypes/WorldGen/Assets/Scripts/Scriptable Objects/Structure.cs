using System;
using UnityEngine;

namespace BaD.Modules.Terrain.Modifiers {
    [CreateAssetMenu()]
    public class Structure: ScriptableObject {

        //public Vector2 texelSize;
        public float radius;
        [Range(0, 1)]
        public float maximumSlope;
        public bool alignToSlope;
        public bool flattenTerrain;
        public int numberToSpawn;
        public GameObject[] structurePrefabs;

        public GameObject GetRandomPrefab ( int seed ) {
            return structurePrefabs[(int) Noise.GetRandomRange(seed, structurePrefabs.Length)];
        }

#if UNITY_EDITOR
        protected void OnValidate () {
            //texelSize.x = Mathf.RoundToInt(texelSize.x);
            //texelSize.y = Mathf.RoundToInt(texelSize.y);
        }
#endif
    }
}