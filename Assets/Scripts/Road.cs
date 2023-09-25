using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class Road : MonoBehaviour, IPathPullable
{
    [SerializeField] private RoadData _roadData;
    [SerializeField] private ObstacleSharedData[] _obstraclesSharedData;
    public RoadSharedData SharedData { get; private set; }

    private void Awake()
    {
        SharedData = new RoadSharedData(GetPath(), _obstraclesSharedData);
    }

    public Vector3[] GetPath()
    {
        Vector3[] path = _roadData.PathMesh.vertices;
        for(uint i = 0; i < path.Length; i++)
        {
            path[i] = transform.TransformPoint(path[i]);
        }

        return path;
    }
}
