using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMove))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerReferenceSO _playerReference;

    private PlayerStats _stats;
    public event Action OnHitPlayer;
    public event Action OnPlayerDeath;
    public event Action OnPlayerDeathComplete;

    private Tween _fallRotateTween;
    private Tween _fallMoveTween;

    private void Awake()
    {
        // PlayerReferenceSO에 자신을 등록
        if (_playerReference != null)
        {
            _playerReference.Current = this;
        }

        _stats = GetComponent<PlayerStats>();
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
        FallPlayer();
        yield return new WaitForSeconds(2f); // 사망 연출 대기
        OnPlayerDeathComplete?.Invoke();
    }

    private void FallPlayer()
    {
        // 오른쪽 or 왼쪽으로 쓰러지기 (90도 회전)
        float fallDirection = UnityEngine.Random.value > 0.5f ? 90f : -90f;

        _fallRotateTween?.Kill();
        _fallRotateTween = transform.DORotate(new Vector3(0, 0, fallDirection), 1f)
            .SetEase(Ease.InQuad);

        // 약간 아래로도 이동 (선택사항)
        _fallMoveTween?.Kill();
        _fallMoveTween = transform.DOMoveY(transform.position.y - 0.5f, 1f)
            .SetEase(Ease.InQuad);
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
