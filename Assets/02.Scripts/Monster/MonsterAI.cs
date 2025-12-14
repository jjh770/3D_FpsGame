using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private EMonsterState _state = EMonsterState.Idle;

    private Player _player;
    private MonsterMove _monsterMove;
    private MonsterStats _stats;
    private MonsterCombat _combat;
    private MonsterPatrol _patrol;

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
        _combat = GetComponent<MonsterCombat>();
        _patrol = GetComponent<MonsterPatrol>();
        _initPosition = transform.position;

        _combat.OnDamageReceived += HandleDamageReceived;
        _combat.OnHealthDepleted += HandleHealthDepleted;
        _combat.OnHit += HandleHit;
        _patrol.OnPatrolCycleComplete += HandlePatrolCycleComplete;
    }
    private void Start()
    {
        StartCoroutine(CheckDistance());
    }
    private void Update()
    {
        if (_monsterMove.IsKnockedBack) return;

        _combat.UpdateAttackTimer(Time.deltaTime);
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
                Hit();
                break;
            case EMonsterState.Death:
                Death();
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
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= _patrolTimer)
        {
            _timer = 0;
            // ResetPatrol() 제거 - 이전 패트롤 위치 유지
            _state = EMonsterState.Patrol;
            Debug.Log($"{gameObject.name} - Idle에서 Patrol로 전환 (이전 위치에서 재개)");
        }
    }

    private void Patrol()
    {
        // 플레이어 감지 시 추적 상태로 전환
        if (_distance <= _stats.DetectDistance.Value)
        {
            _state = EMonsterState.Trace;
            return;
        }

        // 유효한 패트롤 포인트가 있는지 확인
        if (!_patrol.HasValidPatrolPoints())
        {
            Debug.LogWarning($"{gameObject.name} - 패트롤 포인트가 설정되지 않았습니다!");
            _state = EMonsterState.Idle;
            return;
        }

        // MonsterPatrol로부터 현재 목적지 가져오기
        Vector3 destination = _patrol.GetCurrentDestination();

        // 목적지로 이동
        Vector3 direction = (destination - transform.position).normalized;
        _monsterMove.Move(direction, _stats.MoveSpeed.Value * 0.5f); // 패트롤은 절반 속도로

        // 목적지 도달 체크
        if (_patrol.CheckDestinationReached(transform.position))
        {
            _patrol.OnDestinationReached();
        }
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
        if (_distance <= _stats.AttackDistance.Value)
        {
            _combat.ExecuteAttack();
        }
        else
        {
            _state = EMonsterState.Trace;
        }
    }

    private void Hit()
    {
        // Hit 상태에서는 피격 애니메이션 재생 및 이동 중지
        // TODO: Hit 애니메이션 재생
        // 코루틴에서 자동으로 Idle로 전환됨
    }

    private void Death()
    {
        // Death 상태에서는 사망 애니메이션 재생 및 모든 동작 중지
        // TODO: Death 애니메이션 재생
        // 코루틴에서 자동으로 OnDeath 이벤트 발생 → Monster에서 Destroy 처리
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

    private void HandleDamageReceived()
    {
        SetState(EMonsterState.Hit);
        StartCoroutine(_combat.Hit_Coroutine());
    }

    private void HandleHealthDepleted()
    {
        SetState(EMonsterState.Death);
        StartCoroutine(_combat.Death_Coroutine());
    }

    private void HandleHit()
    {
        SetState(EMonsterState.Idle);
    }

    private void HandlePatrolCycleComplete()
    {
        // 패트롤 사이클 완료 (스폰 위치 도착) → Idle로 전환
        Debug.Log($"{gameObject.name} - 패트롤 사이클 완료, Idle로 전환");
        _patrol.ResetPatrol(); // 다음 사이클은 처음부터 시작
        SetState(EMonsterState.Idle);
    }

    private void OnDestroy()
    {
        if (_combat != null)
        {
            _combat.OnDamageReceived -= HandleDamageReceived;
            _combat.OnHealthDepleted -= HandleHealthDepleted;
            _combat.OnHit -= HandleHit;
        }

        if (_patrol != null)
        {
            _patrol.OnPatrolCycleComplete -= HandlePatrolCycleComplete;
        }
    }
}
