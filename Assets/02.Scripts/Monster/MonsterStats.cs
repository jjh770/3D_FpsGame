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

    public ConsumableStat Health => _health;
    public ValueStat DetectDistance => _detectDistance;
    public ValueStat TraceDistance => _traceDistance;
    public ValueStat AttackDistance => _attackDistance;
    public ValueStat MoveSpeed => _moveSpeed;
    public ValueStat AttackCoolTime => _attackCoolTime;
    public ValueStat AttackDamage => _attackDamage;

    private void Start()
    {
        _health.Initialize();
    }

    public float GetHealthPercentage()
    {
        return Health.Value / Health.MaxValue;
    }
}
