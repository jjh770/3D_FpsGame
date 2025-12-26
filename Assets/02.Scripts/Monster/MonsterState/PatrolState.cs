using UnityEngine;

public class PatrolState : MonsterStateBase
{
    public PatrolState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (!_patrol.HasValidPatrolPoints())
        {
            Debug.LogWarning($"{_stateMachine.name} - 패트롤 포인트 없음!");
            _stateMachine.ChangeState(new IdleState(_stateMachine));
            return;
        }

        // 몬스터 스탯에서 이동 속도 설정
        _move.SetMoveSpeed(_stats.MoveSpeed.Value);

        // 즉시 첫 번째 목적지로 이동 시작
        Vector3 destination = _patrol.GetCurrentDestination();
        _move.MoveTo(destination);
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
