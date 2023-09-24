using UnityEngine;

public static class MyMath
{
    public static float DistanceSquare(Vector3 a, Vector3 b)
    {
        float deltaX = b.x - a.x;
        float deltaY = b.y - a.y;
        float deltaZ = b.z - a.z;

        return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
    }

}
