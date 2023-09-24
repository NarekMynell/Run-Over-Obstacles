using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class Road : MonoBehaviour, IPathPullable
{
    [SerializeField] private RoadData _roadData;
    [SerializeField] private ObstacleSharedData[] _obstraclesSharedData;
    public RoadSharedData SharedData { get; private set; }
    public GameObject textPrefab;

    private void Awake()
    {
        SharedData = new RoadSharedData(GetPath(), _obstraclesSharedData);
    }

    private void Start()
    {
        for (int i = 0; i < GetPath().Length; i++)
        {
            //Transform tf = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            //tf.position = GetPath()[i];
            //tf.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Transform text = Instantiate(textPrefab).transform;
            text.position = GetPath()[i];
            text.gameObject.GetComponent<TextMeshPro>().text = i.ToString();
        }
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
