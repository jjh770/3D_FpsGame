using System;
using UnityEngine;

public class MonsterPatrol : MonoBehaviour
{
    [Header("Patrol Path")]
    [SerializeField] private PatrolPathSO _patrolPath;

    private Vector3 _spawnPosition;
    private int _currentWaypointIndex = 0;
    private bool _isReturningToSpawn = false;

    public event Action OnPatrolCycleComplete; // 한 사이클 완료 시 (스폰 위치 도착)

    private void Awake()
    {
        _spawnPosition = transform.position;
    }

    public bool HasValidPatrolPoints()
    {
        return _patrolPath != null && _patrolPath.WaypointCount > 0;
    }

    public Vector3 GetCurrentDestination()
    {
        if (_isReturningToSpawn)
        {
            return _spawnPosition;
        }

        if (_patrolPath == null || _currentWaypointIndex >= _patrolPath.WaypointCount)
        {
            Debug.LogError($"{gameObject.name} - 유효하지 않은 패트롤 경로 또는 인덱스: {_currentWaypointIndex}");
            return _spawnPosition;
        }

        return _patrolPath.GetWaypoint(_currentWaypointIndex);
    }

    public bool CheckDestinationReached(Vector3 currentPosition, float waypointThreshold = 0.5f, float spawnThreshold = 1f)
    {
        Vector3 destination = GetCurrentDestination();
        float distance = Vector3.Distance(currentPosition, destination);

        // 스폰 위치로 복귀 중일 때는 더 큰 threshold 사용
        float threshold = _isReturningToSpawn ? spawnThreshold : waypointThreshold;

        return distance < threshold;
    }

    public void OnDestinationReached()
    {
        if (_isReturningToSpawn)
        {
            // 스폰 위치 도착 - 사이클 완료
            Debug.Log($"{gameObject.name} - 패트롤 사이클 완료 (스폰 위치 도착)");
            _isReturningToSpawn = false;
            OnPatrolCycleComplete?.Invoke();
        }
        else
        {
            // 웨이포인트 도착
            Debug.Log($"{gameObject.name} - 웨이포인트 {_currentWaypointIndex} 도착!");
            _currentWaypointIndex++;

            // 모든 웨이포인트 순회 완료 시 스폰 위치로 복귀
            if (_currentWaypointIndex >= _patrolPath.WaypointCount)
            {
                Debug.Log($"{gameObject.name} - 모든 웨이포인트 순회 완료, 스폰 위치로 복귀");
                _currentWaypointIndex = 0;
                _isReturningToSpawn = true;
            }
        }
    }

    public void ResetPatrol()
    {
        _currentWaypointIndex = 0;
        _isReturningToSpawn = false;
        Debug.Log($"{gameObject.name} - 패트롤 리셋");
    }

    // Gizmo로 패트롤 경로 시각화
    private void OnDrawGizmos()
    {
        if (_patrolPath == null || _patrolPath.WaypointCount == 0)
            return;

        Vector3 spawnPos = Application.isPlaying ? _spawnPosition : transform.position;

        // 스폰 위치
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPos, 0.5f);

        Vector3 prevPos = spawnPos;

        // 웨이포인트 경로 그리기
        for (int i = 0; i < _patrolPath.WaypointCount; i++)
        {
            Vector3 waypointPos = _patrolPath.GetWaypoint(i);

            // 웨이포인트 구체
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(waypointPos, 0.5f);

            // 연결선
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(prevPos, waypointPos);

            prevPos = waypointPos;
        }

        // 마지막 웨이포인트 → 스폰 위치
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(prevPos, spawnPos);
    }
}
