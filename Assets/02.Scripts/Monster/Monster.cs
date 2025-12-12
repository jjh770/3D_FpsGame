using UnityEngine;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterAI))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
public class Monster : MonoBehaviour
{
    [SerializeField] private Player _player;
    private MonsterCombat _combat;
    private MonsterAI _ai;
    private void Awake()
    {
        _combat = GetComponent<MonsterCombat>();
        _ai = GetComponent<MonsterAI>();

        _combat.Initialize(_player);
        _ai.Initialize(_player);

        _combat.OnDeath += HandleDeath;
        _combat.OnHit += HandleHit;
    }
    public void TakeDamage(float damage)
    {
        _combat.TakeDamage(damage);
    }

    public void TakeKnockBack(Vector3 direction, float knockbackAmount)
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
        Debug.Log($"{gameObject.name} 피격!");
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
