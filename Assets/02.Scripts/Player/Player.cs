using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMove_1))]
public class Player : MonoBehaviour, IDamageable
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


    public bool TryTakeDamage(float damage)
    {
        return _stats.Health.TryConsume(damage);
    }
}
