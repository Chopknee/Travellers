using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test: MonoBehaviour {

    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;
    public float displayRadius = 1;
    public int seed = 0;

    List<Vector2> points;

    void OnValidate () {
        points = PoissonDiscSampling.GeneratePoints(seed, radius, regionSize, rejectionSamples);
        Noise.Reset(seed);
    }

    void OnDrawGizmos () {
        Gizmos.DrawWireCube(regionSize / 2, regionSize);
        if (points != null) {
            foreach (Vector2 point in points) {
                Gizmos.DrawSphere(point, displayRadius);
            }
        }
    }
}
