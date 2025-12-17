using System;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    private NavMeshAgent _agent;

    private MonsterCombat _combat;
    private CharacterController _controller;
    private GravityController _gravityController;
    private Animator _animator;
    private Vector3 _knockbackVelocity;
    private bool _wasOnOffMeshLink = false; // 이전 프레임의 OffMeshLink 상태


    public event Action<OffMeshLinkData> OnOffMeshLinkEntered;

    // 최소 넉백 속도 (이거 안넘으면 넉백안됨)
    [SerializeField] private float _minKnockbackVelocity = 0.1f;
    // 넉백 감속 속도
    [SerializeField] private float _knockbackDecay = 5f;
    // 회전 속도
    [SerializeField] private float _rotationSpeed = 10f;
    // 움직임 판정 기준
    [SerializeField] private float _moveSpeedThreshold = 0.1f;

    public bool IsKnockedBack => _knockbackVelocity.magnitude > _minKnockbackVelocity;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<MonsterCombat>();
        _controller = GetComponent<CharacterController>();
        _gravityController = GetComponent<GravityController>();
        _animator = GetComponentInChildren<Animator>();

        _agent.stoppingDistance = 0.2f; // 기본값: 목적지 근처에서 멈춤
        _agent.updateRotation = true; // NavMeshAgent가 자동 회전 처리
        _agent.angularSpeed = _rotationSpeed * 60f; // degrees per second
        _agent.autoTraverseOffMeshLink = false; // OffMeshLink 수동 처리
    }

    private void Update()
    {
        // 넉백 중일 때는 NavMeshAgent 비활성화하고 수동 이동
        if (IsKnockedBack)
        {
            _agent.enabled = false;
            ApplyKnockback();
        }
        else if (!_agent.enabled)
        {
            // 넉백이 끝나면 NavMeshAgent 다시 활성화
            _agent.enabled = true;
        }
        // 움직이는 경우 애니메이션 출력
        UpdateAnimation();

        // OffMeshLink 감지
        if (_agent.enabled)
        {
            CheckOffMeshLink();
        }
    }

    private void UpdateAnimation()
    {
        if (!_agent.enabled) return;

        // NavMeshAgent의 실제 속도로 애니메이션 제어
        float currentSpeed = _agent.velocity.magnitude;
        bool isTracing = currentSpeed > _moveSpeedThreshold;

        // Bool 파라미터
        _animator.SetBool("IsTracing", isTracing);
        _animator.SetFloat("Speed", currentSpeed);
    }

    private void CheckOffMeshLink()
    {
        bool isOnLink = _agent.isOnOffMeshLink;

        // OffMeshLink에 진입한 순간 감지
        if (isOnLink && !_wasOnOffMeshLink)
        {
            OffMeshLinkData linkData = _agent.currentOffMeshLinkData;
            OnOffMeshLinkEntered?.Invoke(linkData);
        }

        _wasOnOffMeshLink = isOnLink;
    }

    // NavMeshAgent를 사용한 목적지 기반 이동
    public void MoveTo(Vector3 destination)
    {
        if (_agent.enabled && !IsKnockedBack)
        {
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }
    }

    public void Stop()
    {
        if (_agent.enabled)
        {
            _agent.isStopped = true;
        }
    }

    public void Resume()
    {
        if (_agent.enabled)
        {
            _agent.isStopped = false;
        }
    }

    public void SetMoveSpeed(float monsterSpeed)
    {
        _agent.speed = monsterSpeed;
    }

    public void AnimTraceToAttack()
    {
        _animator.SetTrigger("TraceToAttackIdle");
    }
    public void AnimAttackToTrace()
    {
        _animator.SetTrigger("AttackIdleToTrace");
    }

    // 특정 위치를 바라보기 (Attack 상태에서 사용)
    public void LookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f; // 수평 방향만

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    public void TakeKnockBack(Vector3 direction, float knockbackAmount)
    {
        _knockbackVelocity = direction.normalized * knockbackAmount;
    }

    private void ApplyKnockback()
    {
        // NavMeshAgent가 비활성화된 상태에서 수동 이동
        transform.position += _knockbackVelocity * Time.deltaTime;

        // 넉백 속도를 점진적으로 감속
        _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _knockbackDecay * Time.deltaTime);
    }

    // NavMeshAgent 속성 접근자
    public float RemainingDistance => _agent.enabled ? _agent.remainingDistance : float.MaxValue;
    public bool HasPath => _agent.enabled && _agent.hasPath;
    public bool PathPending => _agent.enabled && _agent.pathPending;
}
