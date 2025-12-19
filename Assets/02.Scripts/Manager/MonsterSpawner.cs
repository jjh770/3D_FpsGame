using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// 몬스터 스폰을 관리하는 컴포넌트
/// MonsterDataSO를 기반으로 몬스터를 스폰하고 ObjectPool을 사용합니다.
/// </summary>
public class MonsterSpawner : MonoBehaviour
{
    [Header("Monster Types")]
    [SerializeField] private MonsterDataSO[] _monsterTypes;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPoints;
    [Tooltip("true: 랜덤 선택, false: 순차 선택")]
    [SerializeField] private bool _useRandomSpawn = true;

    [Header("Prewarm Settings")]
    [Tooltip("시작 시 풀을 미리 생성할지 여부")]
    [SerializeField] private bool _prewarmOnStart = true;

    [Header("Auto Spawn")]
    [Tooltip("자동으로 몬스터를 스폰할지 여부")]
    [SerializeField] private bool _autoSpawn = false;
    [Tooltip("스폰 간격 (초)")]
    [SerializeField] private float _spawnInterval = 3f;
    [Tooltip("각 몬스터 타입별로 PoolSize만큼 유지 (true) 또는 전체 합산 (false)")]
    [SerializeField] private bool _maintainPerType = true;

    private int _currentSpawnIndex = 0;

    private void Start()
    {
        if (_prewarmOnStart)
        {
            PrewarmPools();
        }

        if (_autoSpawn)
        {
            StartCoroutine(AutoSpawnCoroutine());
        }
    }

    /// <summary>
    /// 모든 몬스터 타입의 풀을 미리 생성합니다.
    /// </summary>
    public void PrewarmPools()
    {
        if (_monsterTypes == null || _monsterTypes.Length == 0)
        {
            Debug.LogWarning("[MonsterSpawner] No monster types assigned!");
            return;
        }

        foreach (var monsterData in _monsterTypes)
        {
            if (monsterData == null || monsterData.Prefab == null)
            {
                Debug.LogWarning($"[MonsterSpawner] Monster data or prefab is null!");
                continue;
            }

            // 몬스터 프리팹 Prewarm
            ObjectPool.Instance.Prewarm(monsterData.Prefab, monsterData.PoolSize);
            Debug.Log($"[MonsterSpawner] Prewarmed {monsterData.PoolSize} {monsterData.name} monsters");

            // VFX 프리팹 Prewarm
            if (monsterData.AttackVFXPrefab != null)
            {
                ObjectPool.Instance.Prewarm(monsterData.AttackVFXPrefab, monsterData.VFXPoolSize);
                Debug.Log($"[MonsterSpawner] Prewarmed {monsterData.VFXPoolSize} {monsterData.name} VFX");
            }
        }
    }

    /// <summary>
    /// 특정 MonsterDataSO를 사용하여 몬스터를 스폰합니다.
    /// </summary>
    public GameObject SpawnMonster(MonsterDataSO monsterData, Vector3 position, Quaternion rotation)
    {
        if (monsterData == null || monsterData.Prefab == null)
        {
            Debug.LogError("[MonsterSpawner] Invalid monster data!");
            return null;
        }

        // ObjectPool에서 가져오기
        GameObject monsterObj = ObjectPool.Instance.Spawn(monsterData.Prefab, position, rotation);

        // IPoolable 초기화
        IPoolable poolable = monsterObj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        // MonsterStats 초기화
        MonsterStats stats = monsterObj.GetComponent<MonsterStats>();
        if (stats != null)
        {
            stats.Initialize(monsterData);
        }

        // MonsterCombat VFX 초기화
        MonsterCombat combat = monsterObj.GetComponent<MonsterCombat>();
        if (combat != null)
        {
            combat.Initialize(monsterData);
        }

        return monsterObj;
    }

    /// <summary>
    /// 특정 MonsterDataSO를 사용하여 몬스터를 스폰합니다. (기본 회전)
    /// </summary>
    public GameObject SpawnMonster(MonsterDataSO monsterData, Vector3 position)
    {
        return SpawnMonster(monsterData, position, Quaternion.identity);
    }

    /// <summary>
    /// 인덱스를 사용하여 몬스터를 스폰합니다.
    /// </summary>
    public GameObject SpawnMonsterByIndex(int index, Vector3 position, Quaternion rotation)
    {
        if (_monsterTypes == null || index < 0 || index >= _monsterTypes.Length)
        {
            Debug.LogError($"[MonsterSpawner] Invalid monster index: {index}");
            return null;
        }

        return SpawnMonster(_monsterTypes[index], position, rotation);
    }

    /// <summary>
    /// 랜덤한 몬스터를 스폰합니다.
    /// </summary>
    public GameObject SpawnRandomMonster(Vector3 position, Quaternion rotation)
    {
        if (_monsterTypes == null || _monsterTypes.Length == 0)
        {
            Debug.LogError("[MonsterSpawner] No monster types assigned!");
            return null;
        }

        int randomIndex = Random.Range(0, _monsterTypes.Length);
        return SpawnMonster(_monsterTypes[randomIndex], position, rotation);
    }

    /// <summary>
    /// 랜덤한 몬스터를 스폰합니다. (기본 회전)
    /// </summary>
    public GameObject SpawnRandomMonster(Vector3 position)
    {
        return SpawnRandomMonster(position, Quaternion.identity);
    }

    // ========== Auto Spawn 기능 ==========

    /// <summary>
    /// 특정 MonsterDataSO의 살아있는 몬스터 개수를 반환합니다.
    /// </summary>
    private int GetAliveMonsterCount(MonsterDataSO monsterData)
    {
        if (monsterData == null || monsterData.Prefab == null)
            return 0;

        // 씬에 있는 모든 Monster 찾기
        Monster[] allMonsters = FindObjectsOfType<Monster>();

        // 활성화되어 있고, 같은 프리팹 이름을 가진 몬스터만 카운트
        return allMonsters.Count(m =>
            m.gameObject.activeSelf &&
            m.gameObject.name == monsterData.Prefab.name
        );
    }

    /// <summary>
    /// 자동 스폰 코루틴
    /// </summary>
    private IEnumerator AutoSpawnCoroutine()
    {
        // 첫 스폰 전 대기 (Prewarm이 완료될 시간)
        yield return new WaitForSeconds(1f);

        while (_autoSpawn)
        {
            if (_monsterTypes == null || _monsterTypes.Length == 0)
            {
                Debug.LogWarning("[MonsterSpawner] No monster types assigned for auto spawn!");
                yield return new WaitForSeconds(_spawnInterval);
                continue;
            }

            if (_maintainPerType)
            {
                // 각 타입별로 PoolSize만큼 유지
                foreach (var monsterData in _monsterTypes)
                {
                    if (monsterData == null) continue;

                    int aliveCount = GetAliveMonsterCount(monsterData);

                    if (aliveCount < monsterData.PoolSize)
                    {
                        GameObject spawned = SpawnAtNextPoint(monsterData);
                        if (spawned != null)
                        {
                            Debug.Log($"[MonsterSpawner] Auto spawned {monsterData.name} ({aliveCount + 1}/{monsterData.PoolSize})");
                        }
                    }
                }
            }
            else
            {
                // 전체 합산해서 관리
                int totalAlive = _monsterTypes.Sum(data => GetAliveMonsterCount(data));
                int totalPoolSize = _monsterTypes.Sum(data => data != null ? data.PoolSize : 0);

                if (totalAlive < totalPoolSize)
                {
                    GameObject spawned = SpawnRandomMonsterAtNextPoint();
                    if (spawned != null)
                    {
                        Debug.Log($"[MonsterSpawner] Auto spawned random monster ({totalAlive + 1}/{totalPoolSize})");
                    }
                }
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    /// <summary>
    /// 자동 스폰을 시작합니다.
    /// </summary>
    public void StartAutoSpawn()
    {
        if (!_autoSpawn)
        {
            _autoSpawn = true;
            StartCoroutine(AutoSpawnCoroutine());
        }
    }

    /// <summary>
    /// 자동 스폰을 중지합니다.
    /// </summary>
    public void StopAutoSpawn()
    {
        _autoSpawn = false;
    }

    // ========== Spawn Point 기반 메서드 ==========

    /// <summary>
    /// 다음 스폰 포인트 위치를 가져옵니다.
    /// </summary>
    private Vector3 GetNextSpawnPosition()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogWarning("[MonsterSpawner] No spawn points assigned! Using spawner position.");
            return transform.position;
        }

        Transform selectedPoint;

        if (_useRandomSpawn)
        {
            // 랜덤 선택
            selectedPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        }
        else
        {
            // 순차 선택 (Round-robin)
            selectedPoint = _spawnPoints[_currentSpawnIndex];
            _currentSpawnIndex = (_currentSpawnIndex + 1) % _spawnPoints.Length;
        }

        return selectedPoint.position;
    }

    /// <summary>
    /// 스폰 포인트를 사용하여 특정 몬스터를 스폰합니다.
    /// </summary>
    public GameObject SpawnAtNextPoint(MonsterDataSO monsterData)
    {
        Vector3 spawnPosition = GetNextSpawnPosition();
        return SpawnMonster(monsterData, spawnPosition);
    }

    /// <summary>
    /// 스폰 포인트를 사용하여 인덱스로 몬스터를 스폰합니다.
    /// </summary>
    public GameObject SpawnMonsterByIndexAtNextPoint(int index)
    {
        if (_monsterTypes == null || index < 0 || index >= _monsterTypes.Length)
        {
            Debug.LogError($"[MonsterSpawner] Invalid monster index: {index}");
            return null;
        }

        Vector3 spawnPosition = GetNextSpawnPosition();
        return SpawnMonster(_monsterTypes[index], spawnPosition);
    }

    /// <summary>
    /// 스폰 포인트를 사용하여 랜덤 몬스터를 스폰합니다.
    /// </summary>
    public GameObject SpawnRandomMonsterAtNextPoint()
    {
        if (_monsterTypes == null || _monsterTypes.Length == 0)
        {
            Debug.LogError("[MonsterSpawner] No monster types assigned!");
            return null;
        }

        Vector3 spawnPosition = GetNextSpawnPosition();
        int randomIndex = Random.Range(0, _monsterTypes.Length);
        return SpawnMonster(_monsterTypes[randomIndex], spawnPosition);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 스폰 포인트 시각화
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            // 스폰 포인트가 없으면 현재 오브젝트 위치 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            return;
        }

        // 각 스폰 포인트 표시
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_spawnPoints[i] == null) continue;

            // 스폰 포인트 구체
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_spawnPoints[i].position, 0.5f);

            // 방향 표시 (화살표)
            Gizmos.color = Color.red;
            Vector3 forward = _spawnPoints[i].forward;
            Gizmos.DrawRay(_spawnPoints[i].position, forward * 1.5f);

            // 번호 표시를 위한 라벨 (Scene View에서만 보임)
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(
                _spawnPoints[i].position + Vector3.up * 0.7f,
                $"Spawn {i}",
                new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                }
            );
            #endif
        }
    }
#endif
}
