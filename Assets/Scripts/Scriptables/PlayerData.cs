using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 2)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _movingSpeed;
    [SerializeField] private float _rotationSpeed;

    public float MovingSpeed => _movingSpeed;
    public float RotationSpeed => _rotationSpeed;
}
