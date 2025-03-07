using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static float DistanceFromLine(this Vector3 point, Vector3 startPos, Vector3 endPos)
    {
        if(startPos == endPos) return Vector3.Distance(point, startPos);
        Vector3 ab = endPos - startPos;
        float lenghtSquare = MyMath.DistanceSquare(startPos, endPos);
        float t = Mathf.Max(0, Mathf.Min(1, Vector3.Dot(point - startPos, ab) / lenghtSquare));
        // Projection falls on the segment
        Vector3 projection = startPos + t * ab;
        float distance = Vector3.Distance(point, projection);
        return distance;
    }

    public static Vector3 PointProjectionOnLine(this Vector3 point, Vector3 startPos, Vector3 endPos)
    {
        if (startPos == endPos) return startPos;
        Vector3 ab = endPos - startPos;
        float lenghtSquare = MyMath.DistanceSquare(startPos, endPos);
        float t = Mathf.Max(0, Mathf.Min(1, Vector3.Dot(point - startPos, ab) / lenghtSquare));
        // Projection falls on the segment
        Vector3 projection = startPos + t * ab;
        return projection;
    }

    public static Vector3 PointProjectionOnVector(this Vector3 point, Vector3 startPos, Vector3 endPos)
    {
        Vector3 directionVector = endPos - startPos;
        // Calculate the projection
        float dotProduct = Vector3.Dot(point, directionVector);
        Vector3 projection = dotProduct / directionVector.sqrMagnitude * directionVector;

        // Calculate the projected point
        Vector3 projectedPointPosition = point + projection;
        return projectedPointPosition;
    }
}
