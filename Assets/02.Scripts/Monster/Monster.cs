using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private EMonsterState _state = EMonsterState.Idle;
    [SerializeField] private GameObject _player;
    [SerializeField] private Player _playerComponent;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private ParticleSystem _attackEffectVFX;
    [SerializeField] private Transform _attackTransform;

    private Vector3 _initPosition;
    private MonsterStats _stats;
    private float _timer = 0f;
    private float _distance;
    private float _distanceToInit;
    private Vector3 _direction;
    private float _checkDistanceInterval = 0.2f;
    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
        _initPosition = transform.position;
    }
    private void Start()
    {
        StartCoroutine(CheckDistance());
    }
    private void Update()
    {
        _timer += Time.deltaTime;

        // 몬스터의 상태에 따라 다른 행동을 한다. (다른 메서드를 호출한다.)
        switch (_state)
        {
            case EMonsterState.Idle:
                Idle();
                break;
            case EMonsterState.Trace:
                Trace();
                break;
            case EMonsterState.Comeback:
                Comeback();
                break;
            case EMonsterState.Attack:
                Attack();
                break;
            case EMonsterState.Hit:
                break;
            case EMonsterState.Death:
                break;

        }
    }

    private void Idle()
    {
        // 대기하는 상태
        // TODO : Idle 애니메이션
        if (_distance <= _stats.DetectDistance.Value)
        {
            _state = EMonsterState.Trace;
        }
    }

    private void Trace()
    {
        // 플레이어를 쫓아가는 상태
        // TODO : Run 애니메이션
        _controller.Move(_direction * _stats.MoveSpeed.Value * Time.deltaTime);

        if (_distance <= _stats.AttackDistance.Value)
        {
            _state = EMonsterState.Attack;
        }

        if (_distance > _stats.DetectDistance.Value)
        {
            _state = EMonsterState.Comeback;
        }
    }

    private void Comeback()
    {
        if (_distanceToInit > 0.5f)
        {
            _controller.Move(_direction * _stats.MoveSpeed.Value * Time.deltaTime);
        }
        else
        {
            _state = EMonsterState.Idle;
        }

        if (_distance <= _stats.DetectDistance.Value)
        {
            _state = EMonsterState.Trace;
        }
    }

    private void Attack()
    {
        // 플레이어를 공격하는 상태
        if (_distance > _stats.AttackDistance.Value)
        {
            _state = EMonsterState.Trace;
            return;
        }

        if (_timer < _stats.AttackCoolTime.Value) return;
        _timer = 0f;
        StartCoroutine(Attack_Coroutine());
    }

    public bool TakeDamage(float damage)
    {
        if (_state == EMonsterState.Hit || _state == EMonsterState.Death)
        {
            return false;
        }
        if (_stats.Health.TryConsume(damage))
        {
            _state = EMonsterState.Hit;
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            _state = EMonsterState.Death;
            StartCoroutine(Death_Coroutine());
        }
        return true;
    }

    private IEnumerator Attack_Coroutine()
    {
        _attackEffectVFX.transform.position = _attackTransform.position;
        _attackEffectVFX.transform.rotation = Quaternion.LookRotation(_direction);
        _attackEffectVFX.Play();
        _playerComponent.TakeDamage(_stats.AttackDamage.Value);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator Hit_Coroutine()
    {
        // TODO : Hit 애니메이션
        yield return new WaitForSeconds(0.2f);
        _state = EMonsterState.Idle;
    }

    private IEnumerator Death_Coroutine()
    {
        // TODO : Death 애니메이션
        //gameObject.transform.
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private IEnumerator CheckDistance()
    {
        while (true)
        {
            yield return new WaitForSeconds(_checkDistanceInterval);

            _distanceToInit = Vector3.Distance(transform.position, _initPosition);
            _distance = Vector3.Distance(transform.position, _player.transform.position);
            if (_state == EMonsterState.Comeback)
            {
                _direction = (_initPosition - transform.position).normalized;
                continue;
            }
            _direction = (_player.transform.position - transform.position).normalized;
        }
    }
}
