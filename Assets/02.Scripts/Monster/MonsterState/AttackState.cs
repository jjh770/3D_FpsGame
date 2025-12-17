public class AttackState : MonsterStateBase
{
    public AttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _move.Stop();
    }

    public override void Update()
    {
        if (_sensor.IsPlayerInRange(_stats.AttackDistance.Value))
        {
            // 플레이어 바라보기
            if (_sensor.Player != null)
            {
                _move.LookAt(_sensor.Player.transform.position);
            }

            // 공격 실행
            _combat.ExecuteAttack();
        }
        else
        {
            // 공격 범위 이탈 → 추적
            _stateMachine.ChangeState(new TraceState(_stateMachine));
        }
    }
}
