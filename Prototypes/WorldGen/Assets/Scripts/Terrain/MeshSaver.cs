using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshSaver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Invoke("SaveMesh", 2);
    }

    void SaveMesh() {

        Map map = OverworldControl.Instance.Map;
        if (map == null) {
            Debug.Log("How the fuck is map null?");
        }
        Texture2D heightmap = new Texture2D(map.heights.GetLength(0), map.heights.GetLength(1), TextureFormat.ARGB32, false);
        for (int x = 0; x < map.heights.GetLength(0); x++) {
            for (int y = 0; y < map.heights.GetLength(1); y++) {
                float val = map.heights[x,y];
                heightmap.SetPixel(x, y, new Color(val, val, val));
            }
        }
        byte[] bytes = heightmap.EncodeToPNG();
        //AssetDatabase.CreateAsset(bytes, "Assets/Models/TestHeightmap.png");
        File.WriteAllBytes("terrainHeightMap.png", bytes);
    }
}
