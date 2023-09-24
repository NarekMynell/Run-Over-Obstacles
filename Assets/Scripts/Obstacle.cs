using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleSharedData _data;

    public ObstacleSharedData Data => _data;
}
