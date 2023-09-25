using System;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    [SerializeField] private Road _road;
    public static event Action<RoadSharedData> OnRoadUpdated;

    private void Start()
    {
        OnRoadUpdated?.Invoke(_road.SharedData);
    }
}