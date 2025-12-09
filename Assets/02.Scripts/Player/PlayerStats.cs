using UnityEngine;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class PlayerStats : MonoBehaviour
{
    // 도메인 별로 나누기
    // 도메인 : 특정 분야의 지식

    // 스태미나 (소모 가능한 스탯)
    [SerializeField] private ConsumableStat _stamina;
    // 체력 (소모 가능한 스탯)
    [SerializeField] private ConsumableStat _health;

    // 각종 스탯 (값 스탯)
    [SerializeField] private ValueStat _damage;
    [SerializeField] private ValueStat _moveSpeed;
    [SerializeField] private ValueStat _sprintSpeed;
    [SerializeField] private ValueStat _jumpPower;

    [SerializeField] private ResourceStat _bombCount;

    public ConsumableStat Stamina => _stamina;
    public ConsumableStat Health => _health;
    public ValueStat Damage => _damage;
    public ValueStat MoveSpeed => _moveSpeed;
    public ValueStat SprintSpeed => _sprintSpeed;
    public ValueStat JumpPower => _jumpPower;
    public ResourceStat BombCount => _bombCount;

    // 스태미나, 체력 관련 코드 (회복, 소모, 업그레이드)
    private void Start()
    {
        _health.Initialize();
        _stamina.Initialize();
        _bombCount.Initialize();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        _health.RegenValue(deltaTime);
        _stamina.RegenValue(deltaTime);
    }
}
