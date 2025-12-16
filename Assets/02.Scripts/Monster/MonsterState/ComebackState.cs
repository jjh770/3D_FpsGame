public class ComebackState : MonsterStateBase
{
    public ComebackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        // 플레이어 재감지
        if (_sensor.IsPlayerInRange(_stats.DetectDistance.Value))
        {
            _stateMachine.ChangeState(new TraceState(_stateMachine));
            return;
        }

        // 스폰 위치로 이동
        if (!_sensor.IsNearSpawn())
        {
            _move.MoveTo(_sensor.SpawnPosition);
        }
        else
        {
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
    }
}
