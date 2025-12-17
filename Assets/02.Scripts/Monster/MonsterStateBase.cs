public interface IMonsterState
{
    void Enter();
    void Update();
    void Exit();
}

public abstract class MonsterStateBase : IMonsterState
{
    protected MonsterStateMachine _stateMachine;
    protected MonsterMove _move;
    protected MonsterCombat _combat;
    protected MonsterStats _stats;
    protected MonsterPatrol _patrol;
    protected MonsterSensor _sensor;
    protected MonsterJump _jump;

    public MonsterStateBase(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _move = stateMachine.Move;
        _combat = stateMachine.Combat;
        _stats = stateMachine.Stats;
        _patrol = stateMachine.Patrol;
        _sensor = stateMachine.Sensor;
        _jump = stateMachine.Jump;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
