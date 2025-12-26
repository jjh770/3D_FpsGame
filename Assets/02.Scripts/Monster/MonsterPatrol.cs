using System;
using UnityEngine;

public class MonsterPatrol : MonoBehaviour
{
    [Header("Patrol Path")]
    [SerializeField] private PatrolPathSO _patrolPath;

    private Vector3 _spawnPosition;
    private int _currentWaypointIndex = 0;
    private bool _isReturningToSpawn = false;
    private bool _hasReachedCurrentDestination = false; // 현재 목적지 도착 여부
    private Vector3 _previousDestination; // 이전 목적지 (플래그 리셋용)

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
        Vector3 destination;

        if (_isReturningToSpawn)
        {
            destination = _spawnPosition;
        }
        else if (_patrolPath == null || _currentWaypointIndex >= _patrolPath.WaypointCount)
        {
            Debug.LogError($"{gameObject.name} - 유효하지 않은 패트롤 경로 또는 인덱스: {_currentWaypointIndex}");
            destination = _spawnPosition;
        }
        else
        {
            destination = _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        // 목적지가 변경되면 도착 플래그 리셋
        if (destination != _previousDestination)
        {
            _hasReachedCurrentDestination = false;
            _previousDestination = destination;
        }

        return destination;
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
        // 이미 이 목적지에 도착 처리했으면 무시
        if (_hasReachedCurrentDestination)
            return;

        _hasReachedCurrentDestination = true;

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

        // 플래그는 GetCurrentDestination()에서 목적지가 변경될 때 자동으로 리셋됨
    }

    public void ResetPatrol()
    {
        _isReturningToSpawn = false;
        _hasReachedCurrentDestination = false;

        // WayPoint0에 이미 있으므로 WayPoint1부터 다시 시작
        // (WayPoint가 1개만 있으면 0으로)
        _currentWaypointIndex = _patrolPath != null && _patrolPath.WaypointCount > 1 ? 1 : 0;

        Debug.Log($"{gameObject.name} - 패트롤 리셋 (다음 목적지: WayPoint{_currentWaypointIndex})");
    }

    /// <summary>
    /// 패트롤 경로를 설정하고 스폰 위치를 업데이트합니다.
    /// </summary>
    public void SetPatrolPath(PatrolPathSO patrolPath)
    {
        _patrolPath = patrolPath;
        _isReturningToSpawn = false;

        // 스폰 위치를 첫 번째 웨이포인트로 설정
        if (_patrolPath != null && _patrolPath.WaypointCount > 0)
        {
            _spawnPosition = _patrolPath.GetWaypoint(0);

            // NavMeshAgent가 있으면 Warp 사용, 없으면 직접 위치 설정
            UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(_spawnPosition);
            }
            else
            {
                transform.position = _spawnPosition;
            }

            // WayPoint0에 스폰했으므로, 다음 목적지는 WayPoint1부터 시작
            // (WayPoint가 1개만 있으면 0으로, 2개 이상이면 1로 설정)
            _currentWaypointIndex = _patrolPath.WaypointCount > 1 ? 1 : 0;
        }
        else
        {
            _currentWaypointIndex = 0;
        }
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
