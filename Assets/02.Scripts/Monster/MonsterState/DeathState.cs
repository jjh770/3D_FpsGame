public class DeathState : MonsterStateBase
{
    public DeathState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _move.Stop();
        _stateMachine.StartCoroutine(_combat.Death_Coroutine());
    }
}
