using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMove_1))]
public class Player : MonoBehaviour
{
    private PlayerMove_1 _move;
    private PlayerGunFire _gunFire;
    private PlayerStats _stats;

    private void Awake()
    {
        _move = GetComponent<PlayerMove_1>();
        _gunFire = GetComponent<PlayerGunFire>();
        _stats = GetComponent<PlayerStats>();
    }


    public void TakeDamage(float damage)
    {
        _stats.Health.TryConsume(damage);
        if (_stats.Health.Value > 0)
        {
            Debug.Log($"플레이어 {damage}입음");
        }
        else
        {
            Debug.Log($"플레이어 사망");
        }

    }
}
