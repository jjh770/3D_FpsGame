using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _explosionEffectPrefab;
    [SerializeField] private float ExplosionRadius = 2;
    [SerializeField] private float Damage = 1000;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 이펙트
        GameObject explosion = ObjectPool.Instance.Spawn(
            _explosionEffectPrefab,
            collision.contacts[0].point,
            Quaternion.identity
        );

        // 2초 후 반환
        ObjectPool.Instance.Despawn(explosion, 2f);

        // 폭탄 반환
        ObjectPool.Instance.Despawn(gameObject);
    }
}

