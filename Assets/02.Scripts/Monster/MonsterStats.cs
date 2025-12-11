using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    [SerializeField] private ConsumableStat _health;

    [SerializeField] private ValueStat _detectDistance;
    [SerializeField] private ValueStat _attackDistance;

    [SerializeField] private ValueStat _moveSpeed;
    [SerializeField] private ValueStat _attackCoolTime;
    [SerializeField] private ValueStat _attackDamage;

    public ConsumableStat Health => _health;
    public ValueStat DetectDistance => _detectDistance;
    public ValueStat AttackDistance => _attackDistance;
    public ValueStat MoveSpeed => _moveSpeed;
    public ValueStat AttackCoolTime => _attackCoolTime;
    public ValueStat AttackDamage => _attackDamage;

    private void Start()
    {
        _health.Initialize();
    }
}
