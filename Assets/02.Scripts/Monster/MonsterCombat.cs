using System;
using System.Collections;
using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [SerializeField] private PlayerReferenceSO _playerReference;
    [SerializeField] private ParticleSystem _attackEffectVFX;
    [SerializeField] private Transform _attackTransform;
    [SerializeField] private float _hitInvincibilityTime = 0.2f;

    private Player _player;
    private MonsterStats _stats;
    private MonsterMove _move;
    private Animator _animator;

    private float _attackTimer = 0f;
    private bool _isInvincible = false; // 무적 상태

    public event Action OnDeath;
    public event Action OnHit;
    public event Action OnDamageReceived;
    public event Action OnHealthDepleted;

    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
        _move = GetComponent<MonsterMove>();
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

    public void AttackVFX()
    {
        Vector3 direction = (_player.transform.position - transform.position).normalized;

        _attackEffectVFX.transform.position = _attackTransform.position;
        _attackEffectVFX.transform.rotation = Quaternion.LookRotation(direction);
        _attackEffectVFX.Play();

        _player.TryTakeDamage(_stats.AttackDamage.Value);
    }

    public void UpdateAttackTimer(float deltaTime)
    {
        _attackTimer += deltaTime;
    }

    public bool TryTakeDamage(float damage)
    {
        // 무적 상태 체크
        if (_isInvincible)
        {
            return false;
        }
        if (_stats.Health.TryConsume(damage))
        {
            OnDamageReceived?.Invoke();
        }
        else
        {
            OnHealthDepleted?.Invoke();
            return false;
        }
        return true;
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
        yield return null;
    }
}
