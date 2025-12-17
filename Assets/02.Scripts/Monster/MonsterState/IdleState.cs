using UnityEngine;

public class IdleState : MonsterStateBase
{
    private float _timer;
    private readonly float _patrolDelay = 1f;

    public IdleState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _timer = 0f;
        _move.Stop();
    }

    public override void Update()
    {
        // 플레이어 감지
        if (_sensor.IsPlayerInRange(_stats.DetectDistance.Value))
        {
            _stateMachine.ChangeState(new TraceState(_stateMachine));
            return;
        }

        // 패트롤 타이머
        _timer += Time.deltaTime;
        if (_timer >= _patrolDelay)
        {
            _stateMachine.ChangeState(new PatrolState(_stateMachine));
        }
    }
}
