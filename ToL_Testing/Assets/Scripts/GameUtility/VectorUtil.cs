using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtil
{
    public static Vector3 ClampPoint(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        return ClampProjection(ProjectPoint(point, segmentStart, segmentEnd), segmentStart, segmentEnd);
    }

    public static Vector3 ProjectPoint(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        return segmentStart + Vector3.Project(point - segmentStart, segmentEnd - segmentStart);
    }

    private static Vector3 ClampProjection(Vector3 point, Vector3 start, Vector3 end)
    {
        var toStart = (point - start).sqrMagnitude;
        var toEnd = (point - end).sqrMagnitude;
        var segment = (start - end).sqrMagnitude;
        if (toStart > segment || toEnd > segment) return toStart > toEnd ? end : start;
        return point;
    }
}
