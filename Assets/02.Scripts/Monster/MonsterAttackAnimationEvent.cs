using UnityEngine;

public class MonsterAttackAnimationEvent : MonoBehaviour
{
    private MonsterCombat _combat;

    private void Awake()
    {
        _combat = GetComponentInParent<MonsterCombat>();
    }

    // AnimationEvent에서 호출
    public void OnAttackHit()
    {
        _combat?.AttackVFX();
    }
}
