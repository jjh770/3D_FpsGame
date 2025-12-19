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

public class Monster : MonoBehaviour, IDamageable, IKnockbackable
{
    private MonsterCombat _combat;
    private MonsterMove _move;
    private Animator _animator;
    private void Awake()
    {
        _combat = GetComponent<MonsterCombat>();
        _move = GetComponent<MonsterMove>();
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
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _move.OnDeathFinish -= HandleDeathFinish;
    }
}
