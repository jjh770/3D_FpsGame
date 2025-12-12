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
    }
    public void TakeDamage(float damage)
    {
        _combat.TakeDamage(damage);
    }

    public void TakeKnockBack(Vector3 direction, float knockbackAmount)
    {
        _combat.TakeKnockback(direction, knockbackAmount);
    }
}
