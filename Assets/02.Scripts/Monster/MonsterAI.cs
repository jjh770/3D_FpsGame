using System.Collections;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private EMonsterState _state = EMonsterState.Idle;

    private Player _player;
    private MonsterMove _monsterMove;
    private MonsterStats _stats;

    private Vector3 _initPosition;
    private float _distance;
    private float _distanceToInit;
    private Vector3 _direction;
    private float _checkDistanceInterval = 0.2f;

    [SerializeField] private float _patrolTimer = 1f;
    private float _timer;

    public EMonsterState State => _state;

    private void Awake()
    {
        _monsterMove = GetComponent<MonsterMove>();
        _stats = GetComponent<MonsterStats>();
        _initPosition = transform.position;
    }
    private void Start()
    {
        StartCoroutine(CheckDistance());
    }
    private void Update()
    {
        if (_monsterMove.IsKnockedBack) return;

        HandleMonsterState();
    }
    public void Initialize(Player player)
    {
        _player = player;
    }
    public void SetState(EMonsterState newState)
    {
        _state = newState;
    }

    private void HandleMonsterState()
    {
        // 몬스터의 상태에 따라 다른 행동을 한다. (다른 메서드를 호출한다.)
        switch (_state)
        {
            case EMonsterState.Idle:
                Idle();
                break;
            case EMonsterState.Patrol:
                Patrol();
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

        if (_timer >= _patrolTimer)
        {
            _timer = 0;
            _state = EMonsterState.Patrol;
        }
    }

    private void Patrol()
    {

    }

    private void Trace()
    {
        // 플레이어를 쫓아가는 상태
        // TODO : Run 애니메이션
        _monsterMove.Move(_direction, _stats.MoveSpeed.Value);

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
            _monsterMove.Move(_direction, _stats.MoveSpeed.Value);
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
        if (_distance > _stats.AttackDistance.Value)
        {
            _state = EMonsterState.Trace;
        }
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
