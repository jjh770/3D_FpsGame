using UnityEngine;

/// <summary>
/// 몬스터의 불변 설정 데이터 (템플릿)
/// 각 몬스터 타입(좀비, 보스 등)의 기본 설정을 정의합니다.
/// </summary>
[CreateAssetMenu(fileName = "New Monster Data", menuName = "Monster System/Monster Data")]
public class MonsterDataSO : ScriptableObject
{
    [Header("Prefab & Pool")]
    [SerializeField] private GameObject _prefab;
    [Tooltip("풀에 미리 생성할 오브젝트 개수")]
    [SerializeField] private int _poolSize = 5;

    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;

    [Header("Detection Distances")]
    [SerializeField] private float _detectDistance = 10f;
    [SerializeField] private float _traceDistance = 20f;
    [SerializeField] private float _attackDistance = 2f;

    [Header("Movement & Combat")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _attackCoolTime = 2f;
    [SerializeField] private float _attackDamage = 10f;

    [Header("VFX")]
    [Tooltip("공격 시 재생할 VFX 프리팹")]
    [SerializeField] private GameObject _attackVFXPrefab;
    [Tooltip("VFX 풀에 미리 생성할 개수")]
    [SerializeField] private int _attackPoolSize = 3;
    [Tooltip("피격 시 재생할 VFX 프리팹")]
    [SerializeField] private GameObject _hitVFXPrefab;
    [Tooltip("VFX 풀에 미리 생성할 개수")]
    [SerializeField] private int _hitPoolSize = 10;

    [Header("Coin Drop")]
    [Tooltip("죽을 때 드랍할 코인 개수")]
    [SerializeField] private int _coinDropCount = 5;


    // Properties
    public GameObject Prefab => _prefab;
    public int PoolSize => _poolSize;
    public float MaxHealth => _maxHealth;
    public float DetectDistance => _detectDistance;
    public float TraceDistance => _traceDistance;
    public float AttackDistance => _attackDistance;
    public float MoveSpeed => _moveSpeed;
    public float AttackCoolTime => _attackCoolTime;
    public float AttackDamage => _attackDamage;
    public GameObject AttackVFXPrefab => _attackVFXPrefab;
    public int AttackPoolSize => _attackPoolSize;
    public GameObject HitVFXPrefab => _hitVFXPrefab;
    public int HitPoolSize => _hitPoolSize;
    public int CoinDropCount => _coinDropCount;
}
