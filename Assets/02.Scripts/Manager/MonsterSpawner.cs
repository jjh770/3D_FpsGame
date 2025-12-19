using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 몬스터 스폰을 관리하는 컴포넌트
/// MonsterDataSO를 기반으로 몬스터를 스폰하고 ObjectPool을 사용합니다.
/// </summary>
public class MonsterSpawner : MonoBehaviour
{
    private static MonsterSpawner _instance;
    public static MonsterSpawner Instance => _instance;

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

    // 살아있는 몬스터 추적 (성능 최적화)
    private Dictionary<string, List<Monster>> _activeMonsters = new Dictionary<string, List<Monster>>();

    private void Awake()
    {
        // Singleton 설정
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

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

    private void Update()
    {
        // Tab 키로 수동 스폰
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SpawnRandomMonsterManually();
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
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

            // VFX 프리팹 Prewarm
            if (monsterData.AttackVFXPrefab != null)
            {
                ObjectPool.Instance.Prewarm(monsterData.AttackVFXPrefab, monsterData.AttackPoolSize);
            }
            if (monsterData.HitVFXPrefab != null)
            {
                ObjectPool.Instance.Prewarm(monsterData.HitVFXPrefab, monsterData.HitPoolSize);
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

        // MonsterVFXController 초기화
        MonsterVFXController vfxController = monsterObj.GetComponent<MonsterVFXController>();
        MonsterCombat combat = monsterObj.GetComponent<MonsterCombat>();
        if (vfxController != null && combat != null)
        {
            vfxController.Initialize(monsterData, combat.AttackTransform);
        }

        // MonsterCombat 상태 리셋
        if (combat != null)
        {
            combat.ResetState();
        }

        // 살아있는 몬스터 리스트에 등록
        Monster monster = monsterObj.GetComponent<Monster>();
        if (monster != null)
        {
            RegisterMonster(monsterData.Prefab.name, monster);

            // 모든 초기화 완료 후 스폰 완료 알림 (HealthBar 등 업데이트)
            monster.NotifySpawnComplete();
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
    /// 몬스터를 살아있는 리스트에 등록합니다.
    /// </summary>
    private void RegisterMonster(string prefabName, Monster monster)
    {
        if (!_activeMonsters.ContainsKey(prefabName))
        {
            _activeMonsters[prefabName] = new List<Monster>();
        }

        if (!_activeMonsters[prefabName].Contains(monster))
        {
            _activeMonsters[prefabName].Add(monster);
        }
    }

    /// <summary>
    /// 몬스터를 살아있는 리스트에서 제거합니다.
    /// </summary>
    public void UnregisterMonster(Monster monster)
    {
        if (monster == null) return;

        string prefabName = monster.name; // 풀에서 나온 오브젝트는 이미 이름이 설정됨

        if (_activeMonsters.ContainsKey(prefabName))
        {
            _activeMonsters[prefabName].Remove(monster);
        }
    }

    /// <summary>
    /// 특정 MonsterDataSO의 살아있는 몬스터 개수를 반환합니다. (최적화됨)
    /// </summary>
    private int GetAliveMonsterCount(MonsterDataSO monsterData)
    {
        if (monsterData == null || monsterData.Prefab == null)
            return 0;

        string prefabName = monsterData.Prefab.name;

        if (!_activeMonsters.ContainsKey(prefabName))
            return 0;

        // 리스트에서 활성화된 몬스터만 카운트 (null 체크 포함)
        return _activeMonsters[prefabName].Count(m => m != null && m.gameObject.activeSelf);
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

    /// <summary>
    /// 수동으로 랜덤 몬스터를 스폰합니다. (Tab 키 또는 외부 호출용)
    /// 풀 제한을 체크하여 PoolSize를 초과하지 않습니다.
    /// </summary>
    private void SpawnRandomMonsterManually()
    {
        if (_monsterTypes == null || _monsterTypes.Length == 0)
        {
            Debug.LogWarning("[MonsterSpawner] No monster types assigned!");
            return;
        }

        // 랜덤 몬스터 타입 선택
        MonsterDataSO randomData = _monsterTypes[Random.Range(0, _monsterTypes.Length)];

        // 풀 제한 체크
        int aliveCount = GetAliveMonsterCount(randomData);
        if (aliveCount >= randomData.PoolSize)
        {
            Debug.LogWarning($"[MonsterSpawner] Cannot spawn {randomData.name}: Pool limit reached ({aliveCount}/{randomData.PoolSize})");
            return;
        }

        // 스폰
        GameObject spawned = SpawnAtNextPoint(randomData);
        if (spawned != null)
        {
            Debug.Log($"[MonsterSpawner] Manually spawned {randomData.name} ({aliveCount + 1}/{randomData.PoolSize})");
        }
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
