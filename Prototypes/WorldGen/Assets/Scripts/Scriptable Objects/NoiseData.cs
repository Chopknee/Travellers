using System;
using UnityEngine;

namespace BaD.Modules {
    [CreateAssetMenu()]
    public class NoiseData: ScriptableObject {
        public float noiseScale;
        public int octaves;
        [Range(0, 1)]
        public float persistance;
        public float lacunarity;
        public int seed;
        public Vector2 offset;
        public Noise.NormalizeMode normalizeMode;

#if UNITY_EDITOR
        public void OnValidate () {
            if (lacunarity < 1) {
                lacunarity = 1;
            }
            if (octaves < 0) {
                octaves = 0;
            }

            //base.OnValidate();
        }
#endif
    }
}
