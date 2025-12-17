public class ReturnToSpawnState : MonsterStateBase
{
    public ReturnToSpawnState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (!_sensor.IsNearSpawn())
        {
            _move.MoveTo(_sensor.SpawnPosition);
        }
    }
}
