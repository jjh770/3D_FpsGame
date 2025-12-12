using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private LayerMask _damageLayer;

    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private float _explosionRadius = 2;
    [SerializeField] private float _damage = 1000;
    [SerializeField] private float _knockbackForce = 100f;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        BombEffect(collision);
        ApplyExplosionDamage();

        // 폭탄 반환
        ObjectPool.Instance.Despawn(gameObject);
    }
    private void ApplyExplosionDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _damageLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                distance = Mathf.Max(1f, distance);

                float finalDamage = _damage / distance;

                damageable.TryTakeDamage(finalDamage);

                if (damageable is IKnockbackable knockbackable)
                {
                    Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;
                    knockbackable.TakeKnockback(knockbackDirection, _knockbackForce);
                }
            }
        }
    }
    private void BombEffect(Collision collision)
    {
        // 폭발 이펙트
        GameObject explosion = ObjectPool.Instance.Spawn(
            _explosionEffectPrefab,
            collision.contacts[0].point,
            Quaternion.identity
        );

        // 2초 후 반환
        ObjectPool.Instance.Despawn(explosion, 2f);
    }
}
