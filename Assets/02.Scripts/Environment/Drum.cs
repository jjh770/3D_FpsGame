using System.Collections;
using UnityEngine;

public class Drum : MonoBehaviour, IDamageable
{
    private Rigidbody _rigidbody;

    [SerializeField] private LayerMask _damageLayer;

    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ValueStat _damage;
    [SerializeField] private ParticleSystem _explosionParticle;

    [SerializeField] private ValueStat _explosionRadius;
    [SerializeField] private ValueStat _explosionForce;
    [SerializeField] private float _knockbackForce = 100f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _health.Initialize();
    }

    public bool TryTakeDamage(float damage)
    {
        if (_health.Value <= 0) return false;

        _health.Decrease(damage);

        if (_health.Value <= 0)
        {
            StartCoroutine(Explode_Coroutine());
        }
        return true;
    }

    private IEnumerator Explode_Coroutine()
    {
        ParticleSystem explosionParticle = Instantiate(_explosionParticle);
        explosionParticle.transform.position = transform.position;
        explosionParticle.Play();

        _rigidbody.AddForce(Vector3.up * _explosionForce.Value);
        _rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * 90f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius.Value, _damageLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage.Value);
            }
            if (collider.TryGetComponent<IKnockbackable>(out var knockbackable))
            {
                Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized;
                knockbackable.TakeKnockback(knockbackDirection, _knockbackForce);
            }
        }

        yield return new WaitForSeconds(3f);
        Destroy(explosionParticle.gameObject, explosionParticle.main.duration);
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius != null ? _explosionRadius.Value : 5f);
    }
}
