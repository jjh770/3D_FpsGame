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
    public bool IsKnockedBack => _knockbackVelocity.magnitude > _minKnockbackVelocity;
    private Vector3 _stateMovement;

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
        //totalMovement += GetStateMovement();
        totalMovement += _stateMovement;

        // 4. 한 번만 Move 호출!
        _controller.Move(totalMovement * Time.deltaTime);
        _stateMovement = Vector3.zero;
    }

    public void Move(Vector3 direction, float speed)
    {
        _stateMovement = direction * speed;
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
}
