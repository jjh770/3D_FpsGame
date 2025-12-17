using UnityEngine;

[RequireComponent(typeof(MonsterMove))]
[RequireComponent(typeof(MonsterStats))]
[RequireComponent(typeof(MonsterCombat))]
[RequireComponent(typeof(MonsterPatrol))]
[RequireComponent(typeof(MonsterJump))]
[RequireComponent(typeof(MonsterSensor))]
public class MonsterStateMachine : MonoBehaviour
{
    // 컴포넌트 참조
    public MonsterMove Move { get; private set; }
    public MonsterStats Stats { get; private set; }
    public MonsterCombat Combat { get; private set; }
    public MonsterPatrol Patrol { get; private set; }
    public MonsterJump Jump { get; private set; }
    public MonsterSensor Sensor { get; private set; }

    private IMonsterState _currentState;

    private void Awake()
    {
        // 컴포넌트 가져오기
        Move = GetComponent<MonsterMove>();
        Stats = GetComponent<MonsterStats>();
        Combat = GetComponent<MonsterCombat>();
        Patrol = GetComponent<MonsterPatrol>();
        Jump = GetComponent<MonsterJump>();
        Sensor = GetComponent<MonsterSensor>();

        // 이벤트 구독
        Combat.OnDamageReceived += HandleDamageReceived;
        Combat.OnHealthDepleted += HandleHealthDepleted;
        Combat.OnHit += HandleHitComplete;
        Patrol.OnPatrolCycleComplete += HandlePatrolComplete;
        Move.OnOffMeshLinkEntered += HandleOffMeshLink;
        Jump.OnJumpComplete += HandleJumpComplete;
        Sensor.OnPlayerDeath += HandlePlayerDeath;
    }

    private void Start()
    {
        ChangeState(new ReadyState(this));
    }

    private void Update()
    {
        // 넉백이나 점프 중에는 상태 업데이트 중지
        if (Move.IsKnockedBack || Jump.IsJumping) return;

        Combat.UpdateAttackTimer(Time.deltaTime);
        _currentState?.Update();
    }

    public void ChangeState(IMonsterState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

#if UNITY_EDITOR
        Debug.Log($"[{gameObject.name}] State Changed: {newState.GetType().Name}");
#endif
    }

    private void HandleDamageReceived()
    {
        ChangeState(new HitState(this));
    }

    private void HandleHealthDepleted()
    {
        ChangeState(new DeathState(this));
    }

    private void HandleHitComplete()
    {
        ChangeState(new IdleState(this));
    }

    private void HandlePatrolComplete()
    {
        Patrol.ResetPatrol();
        ChangeState(new IdleState(this));
    }

    private void HandleOffMeshLink(UnityEngine.AI.OffMeshLinkData linkData)
    {
        Vector3 startPos = linkData.startPos;
        Vector3 endPos = linkData.endPos;

        if (Mathf.Abs(endPos.y - startPos.y) > 0.1f)
        {
            ChangeState(new JumpState(this, linkData));
        }
    }

    private void HandleJumpComplete()
    {
        if (Sensor.IsPlayerInRange(Stats.DetectDistance.Value))
        {
            ChangeState(new TraceState(this));
        }
        else
        {
            ChangeState(new IdleState(this));
        }
    }

    private void HandlePlayerDeath()
    {
        ChangeState(new ReturnToSpawnState(this));
    }

    private void OnDestroy()
    {
        if (Combat != null)
        {
            Combat.OnDamageReceived -= HandleDamageReceived;
            Combat.OnHealthDepleted -= HandleHealthDepleted;
            Combat.OnHit -= HandleHitComplete;
        }

        if (Patrol != null)
        {
            Patrol.OnPatrolCycleComplete -= HandlePatrolComplete;
        }

        if (Move != null)
        {
            Move.OnOffMeshLinkEntered -= HandleOffMeshLink;
        }

        if (Jump != null)
        {
            Jump.OnJumpComplete -= HandleJumpComplete;
        }

        if (Sensor != null)
        {
            Sensor.OnPlayerDeath -= HandlePlayerDeath;
        }
    }
}
