using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimatorController))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMove))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerReferenceSO _playerReference;

    private PlayerStats _stats;
    private PlayerAnimatorController _animatorController;
    public event Action OnHitPlayer;
    public event Action OnPlayerDeath;
    public event Action OnPlayerDeathComplete;
    public event Action<bool> OnActionStateChanged;

    private Tween _fallRotateTween;
    private Tween _fallMoveTween;

    private bool _isPerformingAction = false;

    [SerializeField] private float _deathAnimationDelay = 2f;

    private void Awake()
    {
        // PlayerReferenceSO에 자신을 등록
        if (_playerReference != null)
        {
            _playerReference.Current = this;
        }
        _animatorController = GetComponent<PlayerAnimatorController>();
        _stats = GetComponent<PlayerStats>();
    }

    public void SetActionState(bool isPerforming)
    {
        _isPerformingAction = isPerforming;
        OnActionStateChanged?.Invoke(isPerforming);
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (!_stats.Health.TryConsume(damage.Value))
        {
            StartCoroutine(Death_Coroutine());
            return false;
        }
        OnHitPlayer?.Invoke();
        return true;
    }

    private IEnumerator Death_Coroutine()
    {
        // 플레이어 입력 비활성화
        OnPlayerDeath?.Invoke();
        PlayerDeathAnimation();
        yield return new WaitForSeconds(_deathAnimationDelay); // 사망 연출 대기
        OnPlayerDeathComplete?.Invoke();
    }

    private void PlayerDeathAnimation()
    {
        _animatorController.DeathAnimation();
    }

    private void OnDestroy()
    {
        // PlayerReferenceSO 참조 해제
        if (_playerReference != null && _playerReference.Current == this)
        {
            _playerReference.Current = null;
        }

        _fallRotateTween?.Kill();
        _fallMoveTween?.Kill();
    }
}
