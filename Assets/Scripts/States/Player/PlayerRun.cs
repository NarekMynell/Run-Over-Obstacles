using System.Collections.Generic;
using UnityEngine;

public class PlayerRun : State
{
    protected PlayerController _playerController;
    public PlayerRun(PlayerController playerController) => _playerController = playerController;
    private int _currentPointIndex;
    private float _currentLineLenght;
    private float _movedAlongLine;
    private Vector3 _moveDirection;
    private List<int> _obstaclesNearestSegmentIndex = new();
    private float _upcomingObstacleDistance;


    public override void Enter()
    {
        _currentPointIndex = 0;
        SetNewLine(0);
        InitializePlayerTransform();
        SetObstaclesPointsOnPath();
        _upcomingObstacleDistance = GetDistanceFromUpcomingObstacle();
    }

    public override void FixedUpdate()
    {
        Vector3 moveVector;

        float moveLenght = Time.fixedDeltaTime * _playerController.playerData.MovingSpeed;
        float movAlongLine = _movedAlongLine + moveLenght;
        if(movAlongLine > _currentLineLenght)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _playerController.CurrentRoadData.Path.Length;
            float movAlongNewLine = movAlongLine - _currentLineLenght;
            SetNewLine(movAlongNewLine);
            moveVector = (_playerController.CurrentRoadData.Path[_currentPointIndex] + _moveDirection.normalized * _movedAlongLine - new Vector3(_playerController.transform.position.x, 0, _playerController.transform.position.z));
        }
        else
        {
            moveVector = _moveDirection * moveLenght;
            _movedAlongLine = movAlongLine;
        }

        _playerController.characterController.Move(moveVector);

        // Rotate the object towards the target
        Quaternion targetRotation = Quaternion.LookRotation(moveVector);
        _playerController.transform.rotation = Quaternion.Slerp(_playerController.transform.rotation, targetRotation, _playerController.playerData.RotationSpeed * Time.fixedDeltaTime);


        if(_upcomingObstacleDistance <= 0) _upcomingObstacleDistance = GetDistanceFromUpcomingObstacle();


        float moveDistance = moveVector.magnitude;
        _upcomingObstacleDistance -= moveDistance;
        Debug.Log(_upcomingObstacleDistance);
    }

    private void InitializePlayerTransform()
    {
        Vector3 movVector = _playerController.CurrentRoadData.Path[0] - _playerController.transform.position;
        _playerController.characterController.Move(movVector);
        Vector3 destination = _playerController.CurrentRoadData.Path[1];
        _playerController.transform.LookAt(new Vector3(destination.x, 0, destination.z));
    }

    private void SetNewLine(float moveAlongNewLine)
    {
        int nextPointIndex = (_currentPointIndex + 1) % _playerController.CurrentRoadData.Path.Length;
        Vector3 lineStart = _playerController.CurrentRoadData.Path[_currentPointIndex];
        Vector3 lineEnd = _playerController.CurrentRoadData.Path[nextPointIndex];
        _currentLineLenght = Vector3.Distance(lineStart, lineEnd);
        if(moveAlongNewLine > _currentLineLenght)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _playerController.CurrentRoadData.Path.Length;
            float movAlongNewLine = moveAlongNewLine - _currentLineLenght;
            SetNewLine(movAlongNewLine);
        }
        _movedAlongLine = moveAlongNewLine;
        _moveDirection = (lineEnd - lineStart).normalized;
    }

    private void SetObstaclesPointsOnPath()
    {
        int obstaclesCount = _playerController.CurrentRoadData.Obstacles.Length;
        int pathSegmentsCount = _playerController.CurrentRoadData.Path.Length;

        if (_obstaclesNearestSegmentIndex.Count != obstaclesCount)
        {
            _obstaclesNearestSegmentIndex.Capacity = obstaclesCount;
        }
        _obstaclesNearestSegmentIndex.Clear();

        for (int i = 0; i < obstaclesCount; i++)
        {
            Vector3 obstaclePos = _playerController.CurrentRoadData.Obstacles[i].Position;

            int nearestSegmentStartPointIndex = pathSegmentsCount - 1;
            Vector3 posA = _playerController.CurrentRoadData.Path[nearestSegmentStartPointIndex];
            Vector3 posB = _playerController.CurrentRoadData.Path[0];
            float nearestSegmentDistance = obstaclePos.DistanceFromLine(posA, posB);
            for(int j = 0; j < pathSegmentsCount - 1; j++)
            {
                posA = _playerController.CurrentRoadData.Path[j];
                posB = _playerController.CurrentRoadData.Path[j+1];
                float distance = obstaclePos.DistanceFromLine(posA, posB);
                if(distance < nearestSegmentDistance)
                {
                    nearestSegmentDistance = distance;
                    nearestSegmentStartPointIndex = j;
                }
            }
            _obstaclesNearestSegmentIndex.Add(nearestSegmentStartPointIndex);
        }

        //foreach(int i in _obstaclesNearestSegmentIndex)
        //{
        //    GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = _playerController.CurrentRoadData.Path[i];
        //}
    }

    private float GetDistanceFromUpcomingObstacle()
    {
        float distance = 0f;
        int nextPointIndex = (_currentPointIndex + 1) % _playerController.CurrentRoadData.Path.Length;

        Vector3 currentSegmentStart = _playerController.CurrentRoadData.Path[_currentPointIndex];
        Vector3 currentSegmentEnd = _playerController.CurrentRoadData.Path[nextPointIndex];
        Vector3 posOnPath = _playerController.transform.position.PointProjectionOnLine(currentSegmentStart, currentSegmentEnd);
        float distanceFromCurrentSegmentEnd = Vector3.Distance(posOnPath, currentSegmentEnd);
        distance += distanceFromCurrentSegmentEnd;

        int upcomingObstacleIndex = GetUpcomingObstacleIndex(); Debug.Log(upcomingObstacleIndex);
        int upcomingObstacleSegmentPointIndex = _obstaclesNearestSegmentIndex[upcomingObstacleIndex];
        for(int i = nextPointIndex; i != upcomingObstacleSegmentPointIndex; i = (i+1) % _playerController.CurrentRoadData.Path.Length)
        {
            Vector3 a = _playerController.CurrentRoadData.Path[i];
            Vector3 b = _playerController.CurrentRoadData.Path[(i + 1) % _playerController.CurrentRoadData.Path.Length];
            distance += Vector3.Distance(a, b);
        }

        Vector3 obstacleSegmentStart = _playerController.CurrentRoadData.Path[upcomingObstacleSegmentPointIndex];
        Vector3 obstacleSegmentEnd = _playerController.CurrentRoadData.Path[(upcomingObstacleSegmentPointIndex + 1) % _playerController.CurrentRoadData.Path.Length];
        Vector3 obstaclePos = _playerController.CurrentRoadData.Obstacles[upcomingObstacleIndex].Position;
        Vector3 obstacleProjectionOnSegment = obstaclePos.PointProjectionOnLine(obstacleSegmentStart, obstacleSegmentEnd);
        float distanceFromObstacleSegmentStartToObstacle = Vector3.Distance(obstacleSegmentStart, obstacleProjectionOnSegment);
        distance += distanceFromObstacleSegmentStartToObstacle;



        if(obj!= null) MonoBehaviour.Destroy(obj);
        obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = obstacleProjectionOnSegment;
        obj.transform.localScale = Vector3.one * 0.5f;




        return distance;
    }

    GameObject obj;
    private int GetUpcomingObstacleIndex()
    {
        int smallestPointIndex = _playerController.CurrentRoadData.Path.Length - 1;
        int smallestPointIndexAfterCurrent = _playerController.CurrentRoadData.Path.Length;
        int smallestPointIndexObstacleIndex = default;
        int smallestPointIndexAfterCurrentObstacleIndex = default;
        Debug.Log("currr -------  " + _currentPointIndex);
        for (int i = 0; i < _obstaclesNearestSegmentIndex.Count; i++)
        {
            int segmentPointIndex = _obstaclesNearestSegmentIndex[i];
            Debug.Log(segmentPointIndex);
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

        if(smallestPointIndexAfterCurrent != _playerController.CurrentRoadData.Path.Length)
        {
            return smallestPointIndexAfterCurrentObstacleIndex;
        }
        else
        {
            return smallestPointIndexObstacleIndex;
        }
    }
}