using UnityEngine;

[System.Serializable]
public class ObstacleSharedData
{
    [SerializeField] private Transform _obstacle;
    public Vector3 Position => _obstacle.position;
}