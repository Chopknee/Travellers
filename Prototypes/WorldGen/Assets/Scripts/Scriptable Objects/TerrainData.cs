using System;
using UnityEngine;

namespace BaD.Modules.Terrain {
    [CreateAssetMenu()]
    public class TerrainData: ScriptableObject {
        public float uniformScale = 2.5f;
        public bool useFalloff;
        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public float minHeight {
            get {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
            }
        }

        public float maxHeight {
            get {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
            }
        }
    }
}
