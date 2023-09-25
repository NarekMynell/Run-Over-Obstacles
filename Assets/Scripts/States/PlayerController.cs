using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    public Animator animator;
    public CharacterController characterController;
    public RoadSharedData CurrentRoadData { get; private set; }
    private StateMachine _stateMachine;

    public PlayerData PlayerData => _playerData;

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

    public void OnJumpReadyLeftFoot()
    {
        if(_stateMachine.CurrentState.GetType() == typeof(PlayerRun))
        {
            (_stateMachine.CurrentState as PlayerRun).OnJumpReady(true);
        }
    }

    public void OnJumpReadyRightFoot()
    {
        if (_stateMachine.CurrentState.GetType() == typeof(PlayerRun))
        {
            (_stateMachine.CurrentState as PlayerRun).OnJumpReady(false);
        }
    }

    public void OnJumpEnd()
    {
        if (_stateMachine.CurrentState.GetType() == typeof(PlayerRun))
        {
            (_stateMachine.CurrentState as PlayerRun).OnJumpEnd();
        }
    }
    #endregion
}
