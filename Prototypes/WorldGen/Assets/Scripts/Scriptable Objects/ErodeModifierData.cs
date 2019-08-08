using System;
using UnityEngine;

namespace BaD.Modules.Terrain.Modifiers {
    [CreateAssetMenu()]
    public class ErodeModifierData: AModifierData {

        public ErosionData erosionData;
        public int Iterations = 10000;

        public override void Execute (Map map) {
            if (erosionData == null) {
                return;
            }
            Erosion er = new Erosion(erosionData, map.noiseData.seed);

            float[] mapArray = mapToArray(map.heights);//Create a copy of the map data that the erosion function can use.
            er.Erode(mapArray, (int) map.heights.GetLongLength(0), Iterations, !Application.isPlaying);//Erode the map
                                                                                                       //Copy the data back to the map.
            int i = 0;
            for (int x = 0; x < map.heights.GetLength(0); x++) {
                for (int y = 0; y < map.heights.GetLength(1); y++) {

                    map.heights[x, y] = mapArray[i];
                    i++;
                }
            }
        }

        float[] mapToArray ( float[,] map ) {
            float[] newArray = new float[map.GetLongLength(0) * map.GetLongLength(1)];
            for (int i = 0; i < newArray.Length; i++) {
                newArray[i] = map[i % map.GetLongLength(0), i / map.GetLongLength(0)];
            }
            return newArray;
        }
    }
}
