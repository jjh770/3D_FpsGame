using UnityEngine;

public class AttackState : MonsterStateBase
{
    public AttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _move.Stop();
        _move.AnimTraceToAttack();
    }

    public override void Update()
    {
        _combat.UpdateAttackTimer(Time.deltaTime);

        if (_sensor.IsPlayerInRange(_stats.AttackDistance.Value))
        {
            // 플레이어 바라보기
            if (_sensor.Player != null)
            {
                _move.LookAt(_sensor.Player.transform.position);
            }

            // 공격 실행
            if (_combat.CanAttack())
            {
                _combat.AnimExecuteAttack();
            }
        }
        else
        {
            // 공격 범위 이탈 → 추적
            _stateMachine.ChangeState(new TraceState(_stateMachine));
            _move.AnimAttackToTrace();
        }
    }
}
