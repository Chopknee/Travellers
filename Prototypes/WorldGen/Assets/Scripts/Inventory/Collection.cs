using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu()]
public class Collection: UpdateableData {

    public string collectionName;
    public Color displayColor;

    public void Ass() {
        //interaction.
    }

#if UNITY_EDITOR

    protected override void OnValidate () {

        base.OnValidate();

    }

#endif
}