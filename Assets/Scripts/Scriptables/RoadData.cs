using UnityEngine;

[CreateAssetMenu(fileName = "RoadData", menuName = "ScriptableObjects/RoadData", order = 1)]
public class RoadData : ScriptableObject
{
    [SerializeField] private Mesh _pathMesh;

    public Mesh PathMesh => _pathMesh;
}
