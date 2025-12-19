using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterStateMachine))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(MonsterVFXController))]
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

    public event Action OnMonsterSpawned; // 스폰 완료 후 발생
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

    public bool TryTakeDamage(Damage damage)
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
        // MonsterSpawner에서 등록 해제 (성능 최적화)
        MonsterSpawner.Instance?.UnregisterMonster(this);

        // ObjectPool로 반환
        ObjectPool.Instance.Despawn(gameObject);
    }

    public void OnSpawn()
    {
        // 주의: MonsterStats, MonsterCombat.Initialize()은 MonsterSpawner.SpawnMonster()에서 호출됨
        // 여기서는 컴포넌트 상태만 리셋

        // Move 상태 리셋
        _move?.ResetState();

        // 애니메이터 리셋
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        // StateMachine 초기화 (Start()는 첫 생성 시에만 호출되므로 여기서 리셋 필요)
        _stateMachine?.ChangeState(new ReadyState(_stateMachine));
    }

    /// <summary>
    /// 스폰 완료 후 호출 (MonsterSpawner에서 모든 초기화 완료 후 호출)
    /// </summary>
    public void NotifySpawnComplete()
    {
        OnMonsterSpawned?.Invoke();
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
