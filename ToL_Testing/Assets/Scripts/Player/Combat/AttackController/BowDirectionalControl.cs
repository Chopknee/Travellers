using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BowDirectionalControl : MonoBehaviour
{
    LineRenderer lr;

    public float velocity, angle; // g = gravity value -- radianAngle is the Theta variable.
    public int resolution = 10;
    float g, yawAngle;

    Transform[] directionalArrow = new Transform[2];

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        g = Mathf.Abs(Physics.gravity.y);
    }
    private void Start()
    {
        RenderArc();
    }


    void RenderArc()
    {
        lr.positionCount = (resolution + 1);
        lr.SetPositions(CalculateArcArray());
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        yawAngle = Mathf.Deg2Rad * angle;
        float maxDistance = ((Mathf.Pow(velocity, 2) * Mathf.Sin(2 * yawAngle))) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)(resolution);
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    //calculate height and distance of each vertex in array
    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float dist = t * maxDistance;
        float y = dist * Mathf.Tan(yawAngle) - ((g * dist * dist) / (2 * velocity * Mathf.Cos(yawAngle) * Mathf.Cos(yawAngle)));

        float x = dist * Mathf.Cos(yawAngle);
        float z = dist * Mathf.Sin(yawAngle);

        return new Vector3(x, y, z);
    }


    public Vector3 ProjectileParabola(float v, Vector3 a, Vector3 b)
    {
        // v = velocity, a and b are start and end point

        /*
         * To hit location (x,y) from current location (assuming (0,0))
         * tan(angle) = (v^2 +_ /--v^4 - g(gx^2 - g(gx^2 + 2yv^2)) / (gx)
         * 
        */

        // var tan0 = (v )


        return Vector3.zero;
    }

}
