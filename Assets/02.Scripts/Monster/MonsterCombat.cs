using System;
using System.Collections;
using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [SerializeField] private PlayerReferenceSO _playerReference;
    [SerializeField] private Transform _attackTransform;
    [SerializeField] private float _hitInvincibilityTime = 0.4f;

    private Player _player;
    private MonsterStats _stats;
    private MonsterMove _move;
    private MonsterVFXController _vfxController;
    private Animator _animator;

    private float _attackTimer = 0f;
    private bool _isInvincible = false; // 무적 상태

    public event Action OnDeath;
    public event Action OnHit;
    public event Action OnDamageReceived;
    public event Action OnHealthDepleted;

    public Transform AttackTransform => _attackTransform; // VFXController에서 사용

    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
        _move = GetComponent<MonsterMove>();
        _vfxController = GetComponent<MonsterVFXController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // PlayerReferenceSO에서 Player 가져오기
        if (_playerReference != null)
        {
            _player = _playerReference.Current;
        }
    }

    public bool CanAttack()
    {
        return _attackTimer >= _stats.AttackCoolTime.Value;
    }

    public void AnimExecuteAttack()
    {
        if (!CanAttack()) return;

        _animator.SetTrigger("Attack");
        _attackTimer = 0f;
        //StartCoroutine(Attack_Coroutine());
    }

    /// <summary>
    /// 풀에서 재스폰 시 전투 상태 초기화
    /// </summary>
    public void ResetState()
    {
        _attackTimer = 0f;
        _isInvincible = false;
    }

    public void AttackVFX()
    {
        // Damage 구조체 생성
        Damage damage = new Damage()
        {
            Value = _stats.AttackDamage.Value,
            HitPoint = transform.position,
            Who = gameObject,
            Critical = false
        };

        // VFX 재생은 VFXController에게 위임
        if (_vfxController != null && _player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            _vfxController.PlayAttackVFX(direction);
        }

        // 데미지 처리 (Combat의 책임)
        _player?.TryTakeDamage(damage);
    }

    public void UpdateAttackTimer(float deltaTime)
    {
        _attackTimer += deltaTime;
    }

    public bool TryTakeDamage(Damage damage)
    {
        // 무적 상태 체크
        if (_isInvincible)
        {
            return false;
        }

        // Hit VFX 재생
        _vfxController?.PlayHitVFX(damage.HitPoint);

        if (_stats.Health.TryConsume(damage.Value))
        {
            // 체력이 남았을 때만 HitState로 전환
            OnDamageReceived?.Invoke();
            return true;
        }
        else
        {
            // 체력이 0일 때는 바로 DeathState로 전환
            OnHealthDepleted?.Invoke();
            return false;
        }
    }

    public void TakeKnockback(Vector3 direction, float amount)
    {
        _move.TakeKnockBack(direction, amount);
    }

    public IEnumerator Hit_Coroutine()
    {
        _isInvincible = true;
        OnHit?.Invoke();
        yield return new WaitForSeconds(_hitInvincibilityTime);

        _isInvincible = false;
    }

    public IEnumerator Death_Coroutine()
    {
        OnDeath?.Invoke();
        _animator.SetTrigger("Death");
        yield return null;
    }
}
