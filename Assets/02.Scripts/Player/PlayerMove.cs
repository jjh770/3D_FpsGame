using UnityEngine;

[RequireComponent(typeof(CharacterController))]
// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다
public class PlayerMove : MonoBehaviour
{
    // 필요 속성
    // - 이동속도
    [SerializeField] private float _moveSpeed = 7;
    // - 중력
    [SerializeField] private float _gravity = -9.81f;
    // - 점프력
    [SerializeField] private float _jumpPower = 10f;

    private CharacterController _controller;
    private float _yVelocity = 0f; // 중력에 의해 누적될 값

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 0. 중력을 누적한다.
        _yVelocity += _gravity * Time.deltaTime;

        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // 2. 입력에 따른 방향 구하기
        // 현재는 유니티 세상의 절대적인 방향이 기준 (Global/World 좌표계)
        // 내가 원하는 것은 카메라가 쳐다보는 방향이 기준으로

        // - 글로벌 좌표 방향을 구한다.
        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();

        Debug.Log(_controller.collisionFlags);

        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _yVelocity = _jumpPower;
        }

        // - 카메라가 쳐다보는 방향으로 변환한다. (World -> Local)
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity; // 중력 적용

        // 3. 방향으로 이동시키기 
        // 방향을 따라 강제 이동
        // transform.position += direction * _moveSpeed * Time.deltaTime;
        // Controller가 물리 법칙을 적용하여 이동
        _controller.Move(direction * _moveSpeed * Time.deltaTime);
    }
}
