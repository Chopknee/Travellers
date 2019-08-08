using BaD.Modules.Terrain.Modifiers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BaD.Modules.Terrain {
    [CreateAssetMenu(menuName = "Map Data")]//Ideally this will be modifiable via a gui and generated at runtime, but for now it will be creatable.
    public class MapData: ScriptableObject {
        //General information about the map
        public NoiseData noiseData;
        public TerrainData terrainData;

        public AModifierData[] terrainModifiers;
    }
}
