using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Map))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        Map mapGen = (Map)target;

        if (DrawDefaultInspector()) {
            if (mapGen.UpdateInEditor) {
                mapGen.GenerateInEditor();
            }
        }

        if (GUILayout.Button("Generate")) {
            mapGen.GenerateInEditor();
        }
    }
}
