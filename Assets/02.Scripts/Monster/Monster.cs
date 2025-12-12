using UnityEngine;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterAI))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(MonsterStats))]
public class Monster : MonoBehaviour
{
    private MonsterCombat _combat;
    private void Awake()
    {
        _combat = GetComponent<MonsterCombat>();
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
