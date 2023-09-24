using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    public Animator animator;
    public CharacterController characterController;
    public RoadSharedData CurrentRoadData { get; private set; }
    private StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = new();
    }

    private void OnEnable()
    {
        RunManager.OnRoadUpdated += OnRoadUpdated;
    }

    private void OnDisable()
    {
        RunManager.OnRoadUpdated -= OnRoadUpdated;
    }

    private void Update()
    {
        _stateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.CurrentState.FixedUpdate();
    }

    private void SetRoad(RoadSharedData roadSharedData)
    {
        CurrentRoadData = roadSharedData;
    }

    private void StartRun()
    {
        State state = new PlayerRun(this);
        _stateMachine.Initialize(state);
    }

    #region Event Handlers
    private void OnRoadUpdated(RoadSharedData roadSharedData)
    {
        SetRoad(roadSharedData);
        StartRun();
    }
    #endregion
}
