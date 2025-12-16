using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterAI))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(MonsterPatrol))]
public class Monster : MonoBehaviour, IDamageable, IKnockbackable
{
    [SerializeField] private Transform _mouth;
    private MonsterCombat _combat;
    private MonsterAI _ai;
    private Quaternion _startRotation;
    private Quaternion _mouthRotation = Quaternion.Euler(new Vector3(-60f, 0, 0));
    private float _rotationSpeed = 0.5f;
    private Coroutine _currentMouthCoroutine;

    private void Awake()
    {
        _combat = GetComponent<MonsterCombat>();
        _ai = GetComponent<MonsterAI>();

        _combat.OnDeath += HandleDeath;
        _combat.OnHit += HandleHit;
    }

    private void Start()
    {
        _startRotation = _mouth.transform.localRotation;
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
        Debug.Log($"{gameObject.name} 사망!");
        // 생명주기 관리
        Destroy(gameObject);
    }

    private void HandleHit()
    {
        SetMouthRotation(_mouthRotation);
    }

    private void SetMouthRotation(Quaternion targetRotation)
    {
        if (Quaternion.Angle(_mouth.localRotation, targetRotation) < 0.1f)
            return;

        if (_currentMouthCoroutine != null)
            StopCoroutine(_currentMouthCoroutine);

        _currentMouthCoroutine = StartCoroutine(RotateMouth(targetRotation));
    }

    private IEnumerator RotateMouth(Quaternion targetRotation)
    {
        float elapsed = 0f;
        float duration = 1f / _rotationSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _mouth.localRotation = Quaternion.Slerp(
                _startRotation,
                targetRotation,
                elapsed / duration
            );
            yield return null;
        }

        _mouth.localRotation = targetRotation;
        _currentMouthCoroutine = null;
    }

    private void OnDestroy()
    {
        if (_combat != null)
        {
            _combat.OnDeath -= HandleDeath;
            _combat.OnHit -= HandleHit;
        }
    }
}
