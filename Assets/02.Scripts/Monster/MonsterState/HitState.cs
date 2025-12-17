public class HitState : MonsterStateBase
{
    public HitState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _move.Stop();
        _stateMachine.StartCoroutine(_combat.Hit_Coroutine());
    }
}
