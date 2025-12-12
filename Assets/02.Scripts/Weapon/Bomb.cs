using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private float _explosionRadius = 2;
    [SerializeField] private float _damage = 1000;
    [SerializeField] private float _knockbackForce = 10f;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        BombEffect(collision);
        DamageToMonster();

        // 폭탄 반환
        ObjectPool.Instance.Despawn(gameObject);
    }
    private void DamageToMonster()
    {
        int monsterLayer = LayerMask.NameToLayer("Monster");
        int layerMask = 1 << monsterLayer;

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, layerMask);


        for (int i = 0; i < colliders.Length; i++)
        {
            Monster monster = colliders[i].GetComponent<Monster>();
            if (monster == null) continue;

            float distance = Vector3.Distance(transform.position, monster.transform.position);
            distance = Mathf.Max(1f, distance);

            float finalDamage = _damage / distance;

            monster.TakeDamage(finalDamage);

            Vector3 knockbackDirection = (monster.transform.position - transform.position).normalized;
            monster.TakeKnockback(knockbackDirection, _knockbackForce);
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
