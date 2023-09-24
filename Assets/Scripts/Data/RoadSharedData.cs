using System.Collections;
using UnityEngine;

public class RoadSharedData
{
    public RoadSharedData(Vector3[] path, ObstacleSharedData[] obstacleSharedDatas)
    {
        Path = path;
        Obstacles = obstacleSharedDatas;
    }

    public Vector3[] Path { get; private set; }
    public ObstacleSharedData[] Obstacles { get; private set; }
}