using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMove_1))]
public class Player : MonoBehaviour, IDamageable
{
    private PlayerMove_1 _move;
    private PlayerGunFire _gunFire;
    private PlayerStats _stats;
    public event Action OnHitPlayer;
    public event Action OnPlayerDeath;
    public event Action OnPlayerDeathComplete;
    private void Awake()
    {
        _move = GetComponent<PlayerMove_1>();
        _gunFire = GetComponent<PlayerGunFire>();
        _stats = GetComponent<PlayerStats>();
    }

    public bool TryTakeDamage(float damage)
    {
        if (!_stats.Health.TryConsume(damage))
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
        yield return new WaitForSeconds(2f); // 사망 연출 대기
        OnPlayerDeathComplete?.Invoke();
    }
}
