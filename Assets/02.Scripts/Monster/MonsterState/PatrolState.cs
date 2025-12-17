using UnityEngine;

public class PatrolState : MonsterStateBase
{
    public PatrolState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    [SerializeField] private float _monsterSpeed = 1f;
    public override void Enter()
    {
        if (!_patrol.HasValidPatrolPoints())
        {
            Debug.LogWarning($"{_stateMachine.name} - 패트롤 포인트 없음!");
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
        _move.SetMoveSpeed(_monsterSpeed);
    }

    public override void Update()
    {
        // 플레이어 감지
        if (_sensor.IsPlayerInRange(_stats.DetectDistance.Value))
        {
            _stateMachine.ChangeState(new TraceState(_stateMachine));
            return;
        }

        // 패트롤 이동
        Vector3 destination = _patrol.GetCurrentDestination();
        _move.MoveTo(destination);

        // 목적지 도달 체크
        if (_move.RemainingDistance <= 0.2f && _move.HasPath)
        {
            _patrol.OnDestinationReached();
        }
    }
}
