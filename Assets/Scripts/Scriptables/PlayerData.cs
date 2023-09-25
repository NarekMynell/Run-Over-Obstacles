using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 2)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _runClipDuration;
    [SerializeField] private float _footMaxGapTimePosInJumpAnim;

    public float RunSpeed => _runSpeed;
    public float RotationSpeed => _rotationSpeed;
    public float RunClipDuration => _runClipDuration;
    public float FootMaxGapTimePosInJumpAnim => _footMaxGapTimePosInJumpAnim;
}
