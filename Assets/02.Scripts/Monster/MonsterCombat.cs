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
    private Animator _animator;

    private float _attackTimer = 0f;
    private bool _isInvincible = false; // 무적 상태

    // VFX 풀링용
    private GameObject _attackVFXPrefab;

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

    /// <summary>
    /// MonsterDataSO로 VFX 초기화
    /// </summary>
    public void Initialize(MonsterDataSO data)
    {
        if (data != null)
        {
            _attackVFXPrefab = data.AttackVFXPrefab;
        }

        // 전투 상태 리셋
        ResetState();
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
        Damage damage = new Damage()
        {
            Value = _stats.AttackDamage.Value,
            HitPoint = transform.position,
            Who = gameObject,
            Critical = false
        };

        if (_attackVFXPrefab == null)
        {
            Debug.LogWarning($"[MonsterCombat] Attack VFX prefab is not set for {gameObject.name}");
            _player?.TryTakeDamage(damage);
            return;
        }

        Vector3 direction = (_player.transform.position - transform.position).normalized;
        Vector3 spawnPosition = _attackTransform != null ? _attackTransform.position : transform.position;
        Quaternion spawnRotation = Quaternion.LookRotation(direction);

        // ObjectPool에서 VFX 가져오기
        GameObject vfx = ObjectPool.Instance.Spawn(_attackVFXPrefab, spawnPosition, spawnRotation);

        // ParticleSystem 재생
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();

            // VFX 재생 후 풀로 반환 (duration만큼 후)
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            ObjectPool.Instance.Despawn(vfx, duration);
        }
        else
        {
            // ParticleSystem이 없으면 1초 후 반환
            ObjectPool.Instance.Despawn(vfx, 1f);
        }

        _player?.TryTakeDamage(damage);
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
            OnDamageReceived?.Invoke();
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
        _animator.SetTrigger("Death");
        yield return null;
    }
}
