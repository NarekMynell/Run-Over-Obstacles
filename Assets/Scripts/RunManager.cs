using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    [SerializeField] private Road _road;
    public static event Action<RoadSharedData> OnRoadUpdated;

    //public RoadSharedData CurrentRoadSharedData => _road.SharedData;

    private void Start()
    {
        OnRoadUpdated?.Invoke(_road.SharedData);
    }
}
