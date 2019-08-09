using BaD.Modules.Networking;
using System;
using System.Collections.Generic;
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

        public static byte[] Serialize(object sent) {
            Structure st = (Structure) sent;
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(st.radius));
            bytes.AddRange(BitConverter.GetBytes(st.maximumSlope));
            bytes.Add(Convert.ToByte(st.alignToSlope));
            bytes.Add(Convert.ToByte(st.flattenTerrain));
            bytes.AddRange(BitConverter.GetBytes(st.numberToSpawn));
            bytes.AddRange(BitConverter.GetBytes(st.structurePrefabs.Length));
            //The indices of which structures to spawn.
            foreach (GameObject go in st.structurePrefabs) {
                bytes.AddRange(BitConverter.GetBytes(NetworkInstantiation.Instance.GetSpawnableIndex(go)));
            }
            return bytes.ToArray();
        }

        public object DeSerialize(byte[] received) {
            Structure st = (Structure) ScriptableObject.CreateInstance("Structure");
            st.radius = BitConverter.ToSingle(received, 0);
            st.maximumSlope = BitConverter.ToSingle(received, 4);
            st.alignToSlope = ( received[8] != 0 );
            st.flattenTerrain = ( received[9] != 0 );
            int length = BitConverter.ToInt32(received, 10);
            st.structurePrefabs = new GameObject[length];
            for (int i = 0; i < length; i++) {
                int index = BitConverter.ToInt32(received, 14 + ( i * 4 ));
                st.structurePrefabs[i] = NetworkInstantiation.Instance.SpawnablesPool[index];
            }
            return st;
        }

#if UNITY_EDITOR
        protected void OnValidate () {
            //texelSize.x = Mathf.RoundToInt(texelSize.x);
            //texelSize.y = Mathf.RoundToInt(texelSize.y);
        }
#endif
    }
}