using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterAI))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(MonsterPatrol))]
public class Monster : MonoBehaviour, IDamageable, IKnockbackable
{
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _mouth;
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

        _combat.Initialize(_player);
        _ai.Initialize(_player);

        _ai.SetPatrol += () => SetMouthRotation(_startRotation);
        _combat.OnDeath += HandleDeath;
        _combat.OnHit += () => SetMouthRotation(_mouthRotation);
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
    private void SetMouthRotation(Quaternion targetRotation)
    {
        if (Quaternion.Angle(_mouth.transform.localRotation, targetRotation) < 0.1f)
            return;

        if (_currentMouthCoroutine != null)
            StopCoroutine(_currentMouthCoroutine);

        _currentMouthCoroutine = StartCoroutine(RotateMouth(targetRotation));
    }
    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} 사망!");
        // 생명주기 관리
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (_combat != null)
        {
            _combat.OnDeath -= HandleDeath;
            _combat.OnHit -= () => SetMouthRotation(_mouthRotation);
        }
    }
    private IEnumerator RotateMouth(Quaternion targetRotation)
    {
        Quaternion startRotation = _mouth.transform.localRotation;
        float elapsed = 0f;
        float duration = 1f / _rotationSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _mouth.transform.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                elapsed / duration
            );
            yield return null;
        }

        _mouth.transform.localRotation = targetRotation;
        _currentMouthCoroutine = null;
    }
}
