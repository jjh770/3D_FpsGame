using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private EMonsterState _state = EMonsterState.Idle;
    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;
    private MonsterStats _stats;
    private float _timer = 0f;

    private void Awake()
    {
        _stats = GetComponent<MonsterStats>();
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
        Debug.Log($"{_state}");
        if (Vector3.Distance(transform.position, _player.transform.position) <= _stats.DetectDistance.Value)
        {
            _state = EMonsterState.Trace;
        }
    }

    private void Trace()
    {
        Debug.Log($"{_state}");
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // 플레이어를 쫓아가는 상태
        // TODO : Run 애니메이션
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _controller.Move(direction * _stats.MoveSpeed.Value * Time.deltaTime);

        if (distance <= _stats.DetectDistance.Value)
        {
            _state = EMonsterState.Attack;
        }

    }

    private void Comeback()
    {

    }

    private void Attack()
    {
        // 플레이어를 공격하는 상태
        Debug.Log($"{_state}");
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > _stats.AttackDistance.Value)
        {
            _state = EMonsterState.Trace;
            return;
        }

        if (_timer < _stats.AttackCoolTime.Value) return;
        _timer = 0f;
        // 2. 플레이어 공격하기
        Debug.Log("공격");
    }

    public bool TakeDamage(float damage)
    {
        if (_state == EMonsterState.Hit || _state == EMonsterState.Death)
        {
            return false;
        }
        Debug.Log(_stats.Health.Value);

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
        Debug.Log(_stats.Health.Value);
        return true;
    }

    private IEnumerator Hit_Coroutine()
    {
        // TODO : Hit 애니메이션
        Debug.Log($"{_state}");
        yield return new WaitForSeconds(0.05f);
        _state = EMonsterState.Idle;
    }

    private IEnumerator Death_Coroutine()
    {
        // TODO : Death 애니메이션
        Debug.Log($"{_state}");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
