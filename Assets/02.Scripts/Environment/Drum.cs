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
        ParticleSystem explosionParicle = Instantiate(_explosionParticle);
        explosionParicle.transform.position = transform.position;
        explosionParicle.Play();

        _rigidbody.AddForce(Vector3.up * _explosionForce.Value);
        _rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * 90f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius.Value, _damageLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage.Value);
            }
        }

        yield return new WaitForSeconds(3f);
        Destroy(explosionParicle.gameObject, explosionParicle.main.duration);
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius != null ? _explosionRadius.Value : 5f);
    }
}
