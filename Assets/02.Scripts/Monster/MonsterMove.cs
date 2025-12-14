using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    private CharacterController _controller;
    private GravityController _gravityController;
    private Vector3 _knockbackVelocity;
    // 최소 넉백 속도 (이거 안넘으면 넉백안됨)
    [SerializeField] private float _minKnockbackVelocity = 0.1f;
    // 넉백 감속 속도
    [SerializeField] private float _knockbackDecay = 5f;
    // 회전 속도
    [SerializeField] private float _rotationSpeed = 10f;

    public bool IsKnockedBack => _knockbackVelocity.magnitude > _minKnockbackVelocity;
    private Vector3 _stateMovement;
    private Vector3 _moveDirection; // 이동 방향 저장

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _gravityController = GetComponent<GravityController>();
    }

    private void Update()
    {
        _gravityController.UpdateGravity();
        Vector3 totalMovement = Vector3.zero;

        // 1. 중력
        totalMovement.y = _gravityController.YVelocity;

        // 2. 넉백
        totalMovement += GetKnockbackMovement();

        // 3. 상태별 이동
        totalMovement += _stateMovement;

        // 4. 이동
        _controller.Move(totalMovement * Time.deltaTime);

        // 5. 회전 (이동 방향으로)
        RotateTowards(_moveDirection);

        // 프레임 종료 시 리셋
        _stateMovement = Vector3.zero;
        _moveDirection = Vector3.zero;
    }

    public void Move(Vector3 direction, float speed)
    {
        _stateMovement = direction * speed;
        _moveDirection = direction; // 회전을 위해 방향 저장
    }

    public void TakeKnockBack(Vector3 direction, float knockbackAmount)
    {
        _knockbackVelocity = direction.normalized * knockbackAmount;
    }

    private Vector3 GetKnockbackMovement()
    {
        if (IsKnockedBack)
        {
            var knockbackThisFrame = _knockbackVelocity;
            // 넉백 속도를 점진적으로 감속시킵니다.
            _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _knockbackDecay * Time.deltaTime);
            return knockbackThisFrame;
        }
        return Vector3.zero;
    }

    private void RotateTowards(Vector3 direction)
    {
        // 방향이 없으면 회전하지 않음
        if (direction.sqrMagnitude < 0.01f)
            return;

        // Y축 회전만 (수평 방향)
        Vector3 horizontalDirection = new Vector3(direction.x, 0f, direction.z);

        if (horizontalDirection.sqrMagnitude < 0.01f)
            return;

        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }
}
