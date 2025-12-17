using UnityEngine;

public class ReadyState : MonsterStateBase
{
    private float _timer;
    private readonly float _readyDuration = 3f;

    public ReadyState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _timer = 0f;
        _move.Stop();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _readyDuration)
        {
            _stateMachine.ChangeState(new IdleState(_stateMachine));
        }
    }
}
