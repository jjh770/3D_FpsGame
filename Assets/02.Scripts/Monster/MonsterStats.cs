using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    [SerializeField] private ConsumableStat _health;

    [SerializeField] private ValueStat _detectDistance;
    [SerializeField] private ValueStat _traceDistance;
    [SerializeField] private ValueStat _attackDistance;

    [SerializeField] private ValueStat _moveSpeed;
    [SerializeField] private ValueStat _attackCoolTime;
    [SerializeField] private ValueStat _attackDamage;

    private MonsterDataSO _monsterData;

    public ConsumableStat Health => _health;
    public MonsterDataSO MonsterData => _monsterData;
    public ValueStat DetectDistance => _detectDistance;
    public ValueStat TraceDistance => _traceDistance;
    public ValueStat AttackDistance => _attackDistance;
    public ValueStat MoveSpeed => _moveSpeed;
    public ValueStat AttackCoolTime => _attackCoolTime;
    public ValueStat AttackDamage => _attackDamage;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 기본 초기화 (에디터에서 설정한 값 사용)
    /// </summary>
    public void Initialize()
    {
        _health.Initialize();
    }

    /// <summary>
    /// MonsterDataSO를 사용한 초기화
    /// </summary>
    public void Initialize(MonsterDataSO data)
    {
        if (data == null)
        {
            Initialize();
            return;
        }

        // MonsterDataSO 참조 저장
        _monsterData = data;

        // Health
        _health.SetMaxValue(data.MaxHealth);
        _health.Initialize();

        // Distances
        _detectDistance.SetValue(data.DetectDistance);
        _traceDistance.SetValue(data.TraceDistance);
        _attackDistance.SetValue(data.AttackDistance);

        // Combat
        _moveSpeed.SetValue(data.MoveSpeed);
        _attackCoolTime.SetValue(data.AttackCoolTime);
        _attackDamage.SetValue(data.AttackDamage);
    }

    public float GetHealthPercentage()
    {
        return Health.Value / Health.MaxValue;
    }
}
