using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterJump : MonoBehaviour
{
    private NavMeshAgent _agent;
    private MonsterCombat _combat;
    private Animator _animator;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 2f; // 점프 높이
    [SerializeField] private float _jumpDuration = 1f; // 점프 지속 시간
    [SerializeField] private bool _useAnimationCurve = false; // AnimationCurve 사용 여부
    [SerializeField] private AnimationCurve _jumpCurve; // 포물선 곡선 (0→1→0)

    private bool _isJumping = false;
    private Coroutine _jumpCoroutine;

    public event Action OnJumpComplete;
    public bool IsJumping => _isJumping;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<MonsterCombat>();
        _animator = GetComponentInChildren<Animator>();

        // 기본 포물선 곡선 생성 (AnimationCurve가 설정되지 않았을 때)
        if (_jumpCurve == null || _jumpCurve.keys.Length == 0)
        {
            _jumpCurve = new AnimationCurve(
                new Keyframe(0f, 0f),    // 시작: 높이 0
                new Keyframe(0.5f, 1f),  // 중간: 최대 높이
                new Keyframe(1f, 0f)     // 끝: 높이 0
            );
        }
    }

    public void StartJump(OffMeshLinkData linkData)
    {
        if (_isJumping) return;

        if (_jumpCoroutine != null)
        {
            StopCoroutine(_jumpCoroutine);
        }

        _jumpCoroutine = StartCoroutine(JumpCoroutine(linkData));
    }
    public void AnimTraceToJump()
    {
        _animator.SetTrigger("TraceToJump");
    }

    private IEnumerator JumpCoroutine(OffMeshLinkData linkData)
    {
        _isJumping = true;

        // NavMeshAgent의 자동 위치 업데이트 중지 (수동 제어)
        _agent.updatePosition = false;
        _agent.isStopped = true;

        Vector3 startPos = linkData.startPos;
        Vector3 endPos = linkData.endPos;
        float elapsedTime = 0f;

        // 시작 위치로 이동 (혹시 정확하지 않을 수 있으므로)
        transform.position = startPos;

        // 점프 애니메이션
        while (elapsedTime < _jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / _jumpDuration;

            // 수평 이동 (시작점 → 끝점)
            Vector3 horizontalPos = Vector3.Lerp(startPos, endPos, normalizedTime);

            // 수직 이동 (포물선)
            float heightOffset;
            if (_useAnimationCurve && _jumpCurve != null)
            {
                // AnimationCurve 사용 (0→1→0 형태여야 함)
                heightOffset = _jumpHeight * _jumpCurve.Evaluate(normalizedTime);
            }
            else
            {
                // Sin 곡선 사용 (기본 포물선)
                heightOffset = _jumpHeight * Mathf.Sin(normalizedTime * Mathf.PI);
            }

            // 최종 위치
            transform.position = new Vector3(
                horizontalPos.x,
                Mathf.Lerp(startPos.y, endPos.y, normalizedTime) + heightOffset,
                horizontalPos.z
            );

            yield return null;
        }

        // 최종 위치로 Warp (NavMesh 위치로 정확히 설정)
        _agent.Warp(endPos);

        // NavMeshAgent 설정 복원
        _agent.updatePosition = true;
        _agent.isStopped = false;

        // OffMeshLink 완료 처리
        _agent.CompleteOffMeshLink();

        _isJumping = false;
        _jumpCoroutine = null;

        // 점프 완료 이벤트 발생
        OnJumpComplete?.Invoke();
    }

    // 점프 중단 (필요 시)
    public void CancelJump()
    {
        if (_jumpCoroutine != null)
        {
            StopCoroutine(_jumpCoroutine);
            _jumpCoroutine = null;
        }

        _isJumping = false;

        // NavMeshAgent 설정 복원
        _agent.updatePosition = true;
        _agent.isStopped = false;
    }
}
