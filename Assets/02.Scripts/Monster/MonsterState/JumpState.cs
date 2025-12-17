using UnityEngine.AI;

public class JumpState : MonsterStateBase
{
    private OffMeshLinkData _linkData;

    public JumpState(MonsterStateMachine stateMachine, OffMeshLinkData linkData)
        : base(stateMachine)
    {
        _linkData = linkData;
    }

    public override void Enter()
    {
        _stateMachine.Jump.StartJump(_linkData);
    }
}
