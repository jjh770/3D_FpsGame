public class TraceState : MonsterStateBase
{
    public TraceState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _move.Resume();
    }

    public override void Update()
    {
        // 플레이어 추적
        if (_sensor.Player != null)
        {
            _move.MoveTo(_sensor.Player.transform.position);
        }

        // 공격 범위 진입
        if (_sensor.IsPlayerInRange(_stats.AttackDistance.Value))
        {
            _stateMachine.ChangeState(new AttackState(_stateMachine));
            return;
        }

        // 추적 범위 이탈
        if (!_sensor.IsPlayerInRange(_stats.TraceDistance.Value))
        {
            _stateMachine.ChangeState(new ComebackState(_stateMachine));
        }
    }
}
