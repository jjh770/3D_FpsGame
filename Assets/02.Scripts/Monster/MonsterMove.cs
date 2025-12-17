using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    private MonsterCombat _combat;
    private CharacterController _controller;
    private GravityController _gravityController;
    private Vector3 _knockbackVelocity;
    private bool _wasOnOffMeshLink = false; // 이전 프레임의 OffMeshLink 상태

    public event Action<OffMeshLinkData> OnOffMeshLinkEntered;
    public event Action OnDeathFinish;

    // 최소 넉백 속도 (이거 안넘으면 넉백안됨)
    [SerializeField] private float _minKnockbackVelocity = 0.1f;
    // 넉백 감속 속도
    [SerializeField] private float _knockbackDecay = 5f;
    // 회전 속도
    [SerializeField] private float _rotationSpeed = 10f;

    private Tween _fallRotateTween;
    private Tween _fallMoveTween;

    public bool IsKnockedBack => _knockbackVelocity.magnitude > _minKnockbackVelocity;
    private bool _isDead = false;
    private float _fallTime = 2f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<MonsterCombat>();
        _controller = GetComponent<CharacterController>();
        _gravityController = GetComponent<GravityController>();

        _agent.stoppingDistance = 0.2f; // 기본값: 목적지 근처에서 멈춤
        _agent.updateRotation = true; // NavMeshAgent가 자동 회전 처리
        _agent.angularSpeed = _rotationSpeed * 60f; // degrees per second
        _agent.autoTraverseOffMeshLink = false; // OffMeshLink 수동 처리
    }
    private void Start()
    {
        _combat.OnDeath += HandleDeathStart;
    }
    private void Update()
    {
        if (_isDead) return;

        // 넉백 중일 때는 NavMeshAgent 비활성화하고 수동 이동
        if (IsKnockedBack)
        {
            if (_agent.enabled)
            {
                _agent.enabled = false; // NavMesh 제어 끔
            }
            ApplyKnockback();
        }
        else if (!_agent.enabled)
        {
            // 넉백이 끝나면 NavMeshAgent 다시 활성화
            _agent.enabled = true;
            _agent.Warp(transform.position);
        }

        // OffMeshLink 감지
        if (_agent.enabled)
        {
            CheckOffMeshLink();
        }
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
        direction.y = 0f;
        _knockbackVelocity = direction.normalized * knockbackAmount;
    }

    private void ApplyKnockback()
    {
        // 중력 업데이트
        _gravityController.UpdateGravity();

        // 넉백 이동 (수평) + 중력 (수직)
        Vector3 movement = _knockbackVelocity * Time.deltaTime;
        movement.y = _gravityController.YVelocity * Time.deltaTime;

        // CharacterController로 이동 (충돌 감지 + 바닥 체크 자동)
        _controller.Move(movement);

        // 수평 속도만 감속
        Vector3 horizontal = new Vector3(_knockbackVelocity.x, 0f, _knockbackVelocity.z);
        horizontal = Vector3.Lerp(horizontal, Vector3.zero, _knockbackDecay * Time.deltaTime);
        _knockbackVelocity.x = horizontal.x;
        _knockbackVelocity.z = horizontal.z;

        //// NavMeshAgent가 비활성화된 상태에서 수동 이동
        //transform.position += _knockbackVelocity * Time.deltaTime;

        //// 넉백 속도를 점진적으로 감속
        //_knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _knockbackDecay * Time.deltaTime);
    }

    private void HandleDeathStart()
    {
        _isDead = true;
        _agent.enabled = false;
        _knockbackVelocity = Vector3.zero;
        StartCoroutine(FallMonster());
    }

    private IEnumerator FallMonster()
    {
        // 오른쪽 or 왼쪽으로 쓰러지기 (90도 회전)
        float fallDirection = UnityEngine.Random.value > 0.5f ? 90f : -90f;

        _fallRotateTween?.Kill();
        _fallRotateTween = transform.DORotate(new Vector3(0, 0, fallDirection), 1f)
            .SetEase(Ease.InQuad);

        // 약간 아래로도 이동 (선택사항)
        _fallMoveTween?.Kill();
        _fallMoveTween = transform.DOMoveY(transform.position.y - 0.5f, 1f)
            .SetEase(Ease.InQuad);

        float fallTime = 0f;
        while (fallTime < _fallTime)
        {
            // 중력 업데이트
            _gravityController.UpdateGravity();

            // 넉백 이동 (수평) + 중력 (수직)
            Vector3 movement = _knockbackVelocity * Time.deltaTime;
            movement.y = _gravityController.YVelocity * Time.deltaTime;

            // CharacterController로 이동 (충돌 감지 + 바닥 체크 자동)
            _controller.Move(movement);
            fallTime += Time.deltaTime;

            yield return null; // 다음 프레임까지 대기
        }
        OnDeathFinish?.Invoke();
    }

    // NavMeshAgent 속성 접근자
    public float RemainingDistance => _agent.enabled ? _agent.remainingDistance : float.MaxValue;
    public bool HasPath => _agent.enabled && _agent.hasPath;
    public bool PathPending => _agent.enabled && _agent.pathPending;
}
