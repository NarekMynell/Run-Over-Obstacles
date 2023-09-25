using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : State
{
    public PlayerRun(PlayerController playerController)
    {
        _playerController = playerController;
        _speed = _playerController.PlayerData.RunSpeed;
        _runClipCycleMov = _speed * _playerController.PlayerData.RunClipDuration;
    }

    private const string _leftFootJumpTrigger = "JumpFromLeftFoot";
    private const string _rightFootJumpTrigger = "JumpFromRightFoot";
    protected PlayerController _playerController;
    private float _speed;
    private int _currentPointIndex;
    private float _currentLineLenght;
    private float _movedAlongLine;
    private Vector3 _moveDirection;
    private List<int> _obstaclesNearestSegmentIndex = new();
    private float _upcomingObstacleDistance;
    private float _runClipCycleMov;
    private Vector3[] Path => _playerController.CurrentRoadData.Path;
    private ObstacleSharedData[] Obstacles => _playerController.CurrentRoadData.Obstacles;


    public override void Enter()
    {
        _currentPointIndex = 0;
        SetNewLine(0);
        InitializePlayerTransform();
        UpdateObstaclesData();
    }

    public override void Update()
    {
        CheckObstaclesChange();

        Vector3 moveVector;

        float moveLenght = Time.deltaTime * _speed;
        float movAlongLine = _movedAlongLine + moveLenght;
        if(movAlongLine > _currentLineLenght)
        {
            _currentPointIndex = (_currentPointIndex + 1) % Path.Length;
            float movAlongNewLine = movAlongLine - _currentLineLenght;
            SetNewLine(movAlongNewLine);
            moveVector = (Path[_currentPointIndex] + _moveDirection.normalized * _movedAlongLine - new Vector3(_playerController.transform.position.x, 0, _playerController.transform.position.z));
        }
        else
        {
            moveVector = _moveDirection * moveLenght;
            _movedAlongLine = movAlongLine;
        }

        _playerController.characterController.Move(moveVector);

        // Rotate the object towards the target
        Quaternion targetRotation = Quaternion.LookRotation(moveVector);
        _playerController.transform.rotation = Quaternion.Slerp(_playerController.transform.rotation, targetRotation, _playerController.PlayerData.RotationSpeed * Time.deltaTime);

        float moveDistance = moveVector.magnitude;
        _upcomingObstacleDistance -= moveDistance;

        if (_upcomingObstacleDistance <= 0) _upcomingObstacleDistance = GetDistanceFromUpcomingObstacle();
    }

    private void InitializePlayerTransform()
    {
        Vector3 movVector = Path[0] - _playerController.transform.position;
        _playerController.characterController.Move(movVector);
        Vector3 destination = Path[1];
        _playerController.transform.LookAt(new Vector3(destination.x, 0, destination.z));
    }

    private void SetNewLine(float moveAlongNewLine)
    {
        int nextPointIndex = (_currentPointIndex + 1) % Path.Length;
        Vector3 lineStart = Path[_currentPointIndex];
        Vector3 lineEnd = Path[nextPointIndex];
        _currentLineLenght = Vector3.Distance(lineStart, lineEnd);
        // TO DO
        // if(moveAlongNewLine > _currentLineLenght) need recursion

        _movedAlongLine = moveAlongNewLine;
        _moveDirection = (lineEnd - lineStart).normalized;
    }

    private void SetObstaclesPointsOnPath()
    {
        int obstaclesCount = Obstacles.Length;
        int pathSegmentsCount = Path.Length;

        if (_obstaclesNearestSegmentIndex.Count != obstaclesCount)
        {
            _obstaclesNearestSegmentIndex.Capacity = obstaclesCount;
        }
        _obstaclesNearestSegmentIndex.Clear();

        for (int i = 0; i < obstaclesCount; i++)
        {
            Vector3 obstaclePos = Obstacles[i].Position;

            int nearestSegmentStartPointIndex = pathSegmentsCount - 1;
            Vector3 posA = Path[nearestSegmentStartPointIndex];
            Vector3 posB = Path[0];
            float nearestSegmentDistance = obstaclePos.DistanceFromLine(posA, posB);
            for(int j = 0; j < pathSegmentsCount - 1; j++)
            {
                posA = Path[j];
                posB = Path[j+1];
                float distance = obstaclePos.DistanceFromLine(posA, posB);
                if(distance < nearestSegmentDistance)
                {
                    nearestSegmentDistance = distance;
                    nearestSegmentStartPointIndex = j;
                }
            }
            _obstaclesNearestSegmentIndex.Add(nearestSegmentStartPointIndex);
        }
    }

    private Vector3[] _obstaclesLastPos;
    private void SaveObstaclePoses()
    {
        _obstaclesLastPos = new Vector3[Obstacles.Length];

        for(int i = 0; i < _obstaclesLastPos.Length; i++)
        {
            _obstaclesLastPos[i] = Obstacles[i].Position;
        }
    }

    private void CheckObstaclesChange()
    {
        if(_obstaclesLastPos.Length != Obstacles.Length)
        {
            UpdateObstaclesData();
            return;
        }

        for(int i = 0; i < _obstaclesLastPos.Length;i++)
        {
            if(_obstaclesLastPos[i] != Obstacles[i].Position)
            {
                UpdateObstaclesData();
                break;
            }
        }
    }

    private void UpdateObstaclesData()
    {
        SetObstaclesPointsOnPath();
        SaveObstaclePoses();
        _upcomingObstacleDistance = GetDistanceFromUpcomingObstacle();
    }

    private float GetDistanceFromUpcomingObstacle()
    {
        float distance = 0f;
        int nextPointIndex = (_currentPointIndex + 1) % Path.Length;

        Vector3 currentSegmentStart = Path[_currentPointIndex];
        Vector3 currentSegmentEnd = Path[nextPointIndex];
        Vector3 posOnPath = _playerController.transform.position.PointProjectionOnLine(currentSegmentStart, currentSegmentEnd);
        float distanceFromCurrentSegmentEnd = Vector3.Distance(posOnPath, currentSegmentEnd);
        distance += distanceFromCurrentSegmentEnd;

        int upcomingObstacleIndex = GetUpcomingObstacleIndex();
        int upcomingObstacleSegmentPointIndex = _obstaclesNearestSegmentIndex[upcomingObstacleIndex];
        for(int i = nextPointIndex; i != upcomingObstacleSegmentPointIndex; i = (i+1) % Path.Length)
        {
            Vector3 a = Path[i];
            Vector3 b = Path[(i + 1) % Path.Length];
            distance += Vector3.Distance(a, b);
        }

        Vector3 obstacleSegmentStart = Path[upcomingObstacleSegmentPointIndex];
        Vector3 obstacleSegmentEnd = Path[(upcomingObstacleSegmentPointIndex + 1) % Path.Length];
        Vector3 obstaclePos = Obstacles[upcomingObstacleIndex].Position;
        Vector3 obstacleProjectionOnSegment = obstaclePos.PointProjectionOnLine(obstacleSegmentStart, obstacleSegmentEnd);
        float distanceFromObstacleSegmentStartToObstacle = Vector3.Distance(obstacleSegmentStart, obstacleProjectionOnSegment);
        distance += distanceFromObstacleSegmentStartToObstacle;
        return distance;
    }

    private int GetUpcomingObstacleIndex()
    {
        int smallestPointIndex = Path.Length - 1;
        int smallestPointIndexAfterCurrent = Path.Length;
        int smallestPointIndexObstacleIndex = default;
        int smallestPointIndexAfterCurrentObstacleIndex = default;
        for (int i = 0; i < _obstaclesNearestSegmentIndex.Count; i++)
        {
            int segmentPointIndex = _obstaclesNearestSegmentIndex[i];
            if (segmentPointIndex > _currentPointIndex && segmentPointIndex < smallestPointIndexAfterCurrent)
            {
                smallestPointIndexAfterCurrent = segmentPointIndex;
                smallestPointIndexAfterCurrentObstacleIndex = i;
            }
            else if(segmentPointIndex < smallestPointIndex)
            {
                smallestPointIndex = segmentPointIndex;
                smallestPointIndexObstacleIndex = i;
            }
        }

        if(smallestPointIndexAfterCurrent != Path.Length)
        {
            return smallestPointIndexAfterCurrentObstacleIndex;
        }
        else
        {
            return smallestPointIndexObstacleIndex;
        }
    }

    public void OnJumpReady(bool isLeftFoot)
    {
        float footsCycleDistance = _runClipCycleMov / 2;
        // 0.2 is offset so that the flight does not start too close to the obstacle
        if (_upcomingObstacleDistance - 0.2f < footsCycleDistance)
        {
            float distanceToFootsMaxGap = _speed * _playerController.PlayerData.FootMaxGapTimePosInJumpAnim;
            float speed = distanceToFootsMaxGap / _upcomingObstacleDistance;
            _playerController.animator.speed = speed;
            _playerController.animator.SetTrigger((isLeftFoot ? _leftFootJumpTrigger : _rightFootJumpTrigger));
        }
    }

    public void OnJumpEnd()
    {
        _playerController.animator.speed = 1;
    }
}