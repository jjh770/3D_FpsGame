using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterStateMachine))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(MonsterPatrol))]
[RequireComponent(typeof(MonsterJump))]
[RequireComponent(typeof(MonsterSensor))]
[RequireComponent(typeof(NavMeshAgent))]

public class Monster : MonoBehaviour, IDamageable, IKnockbackable, IPoolable
{
    private MonsterCombat _combat;
    private MonsterMove _move;
    private MonsterStats _stats;
    private MonsterStateMachine _stateMachine;
    private Animator _animator;

    private void Awake()
    {
        _combat = GetComponent<MonsterCombat>();
        _move = GetComponent<MonsterMove>();
        _stats = GetComponent<MonsterStats>();
        _stateMachine = GetComponent<MonsterStateMachine>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _move.OnDeathFinish += HandleDeathFinish;
    }

    public bool TryTakeDamage(float damage)
    {
        return _combat.TryTakeDamage(damage);
    }

    public void TakeKnockback(Vector3 direction, float knockbackAmount)
    {
        _animator.SetTrigger("Hit");
        _combat.TakeKnockback(direction, knockbackAmount);
    }

    private void HandleDeathFinish()
    {
        ObjectPool.Instance.Despawn(gameObject);
    }

    public void OnSpawn()
    {
        // 스탯 초기화 (MonsterStats에서 처리)
        _stats?.Initialize();

        // 애니메이터 리셋
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        // StateMachine은 자동으로 Start에서 ReadyState로 시작
    }

    public void OnDespawn()
    {
        // 이벤트 정리는 이미 OnDestroy에서 처리되므로 여기선 불필요
        // 필요시 추가 정리 로직 작성
    }

    private void OnDestroy()
    {
        if (_move != null)
        {
            _move.OnDeathFinish -= HandleDeathFinish;
        }
    }
}
