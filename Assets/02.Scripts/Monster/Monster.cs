using DG.Tweening;
using System.Collections;
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

    private Tween _fallRotateTween;
    private Tween _fallMoveTween;

    private void Awake()
    {
        _combat = GetComponent<MonsterCombat>();
        _combat.OnDeath += HandleDeath;
    }

    public bool TryTakeDamage(float damage)
    {
        return _combat.TryTakeDamage(damage);
    }

    public void TakeKnockback(Vector3 direction, float knockbackAmount)
    {
        _combat.TakeKnockback(direction, knockbackAmount);
    }

    private void HandleDeath()
    {
        StartCoroutine(FallMonster());
    }

    private IEnumerator FallMonster()
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

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_combat != null)
        {
            _combat.OnDeath -= HandleDeath;
        }
    }
}
