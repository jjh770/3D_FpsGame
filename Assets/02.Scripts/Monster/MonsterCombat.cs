using System.Collections;
using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [SerializeField] private ParticleSystem _attackEffectVFX;
    [SerializeField] private Transform _attackTransform;

    private Player _player;
    private MonsterStats _stats;
    private MonsterAI _ai;
    private MonsterMove _move;
    private float _attackTimer = 0f;

    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
        _ai = GetComponent<MonsterAI>();
        _move = GetComponent<MonsterMove>();
    }

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_ai.State == EMonsterState.Attack)
        {
            TryAttack();
        }
    }
    public void Initialize(Player player)
    {
        _player = player;
    }
    private void TryAttack()
    {
        if (_attackTimer < _stats.AttackCoolTime.Value) return;

        _attackTimer = 0f;
        StartCoroutine(Attack_Coroutine());
    }

    private IEnumerator Attack_Coroutine()
    {
        Vector3 direction = (_player.transform.position - transform.position).normalized;

        _attackEffectVFX.transform.position = _attackTransform.position;
        _attackEffectVFX.transform.rotation = Quaternion.LookRotation(direction);
        _attackEffectVFX.Play();
        _player.TakeDamage(_stats.AttackDamage.Value);

        yield return new WaitForSeconds(1f);
    }

    public bool TakeDamage(float damage)
    {
        if (_ai.State == EMonsterState.Hit || _ai.State == EMonsterState.Death)
        {
            return false;
        }

        if (_stats.Health.TryConsume(damage))
        {
            _ai.SetState(EMonsterState.Hit);
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            _ai.SetState(EMonsterState.Death);
            StartCoroutine(Death_Coroutine());
        }
        return true;
    }

    public void TakeKnockback(Vector3 direction, float amount)
    {
        _move.TakeKnockBack(direction, amount);
    }

    private IEnumerator Hit_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);
        _ai.SetState(EMonsterState.Comeback);
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
