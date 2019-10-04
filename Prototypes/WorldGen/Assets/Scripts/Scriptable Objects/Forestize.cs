using System.Collections.Generic;
using UnityEngine;

namespace BaD.Modules.Terrain.Modifiers {
    [CreateAssetMenu()]
    public class Forestize: AModifierData {

        public float SpawnRadius = 1;

        public List<GameObject> scatterObjects;

        private Vector2 sampleRegionSize;
        private int seed;
        private float cellSize;
        List<Vector2> points;
        List<Vector2> spawnPoints;
        int[,] poissonGrid;
        public float numSamplesBeforeRejection = 30;

        public override void Execute ( Map map ) {
            List<Vector2> points = PoissonDiscSampling.GeneratePoints(map.noiseData.seed, SpawnRadius, new Vector2(map.mapChunkSize, map.mapChunkSize));

            foreach (Vector2 pt in points) {
                //Select a new point here...
            }
        }
    }
}
