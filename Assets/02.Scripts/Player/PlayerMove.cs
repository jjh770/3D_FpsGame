using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다
public class PlayerMove : MonoBehaviour
{
    // 필요 속성
    // - 이동속도
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _sprintSpeed = 12f;
    private float _playerSpeed;
    // - 중력
    [SerializeField] private float _gravity = -9.81f;
    // - 점프력
    [SerializeField] private float _jumpPower = 10f;
    [SerializeField] private float _doubleJumpCost = 10f;
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _sprintCost = 10f;
    [SerializeField] private float _gainStaminaAmount = 5f;
    [SerializeField] private float _gainStaminaDelay = 1f;


    private CharacterController _controller;
    private float _yVelocity = 0f; // 중력에 의해 누적될 값
    private float _currentStamina;
    private bool _isSprinting = false;
    private float _lastStaminaUseTime;
    private int _jumpCount = 0;

    public float MaxStamina => _maxStamina;
    public float CurrentStamina => _currentStamina;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _currentStamina = _maxStamina;
    }

    private void Update()
    {
        GravityPlayer();
        MovePlayer();
        JumpPlayer();
        StaminaPlayer();
    }

    private void GravityPlayer()
    {
        // 0. 중력을 누적한다.
        _yVelocity += _gravity * Time.deltaTime;
    }

    private void MovePlayer()
    {
        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // 2. 입력에 따른 방향 구하기
        // 현재는 유니티 세상의 절대적인 방향이 기준 (Global/World 좌표계)
        // 내가 원하는 것은 카메라가 쳐다보는 방향이 기준으로

        // - 글로벌 좌표 방향을 구한다.
        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();

        _playerSpeed = _moveSpeed;

        if ((Input.GetKey(KeyCode.LeftShift) && direction.magnitude > 0.1f) && _currentStamina > 0f)
        {
            _isSprinting = true;
            _playerSpeed = _sprintSpeed;
        }
        else
        {
            _isSprinting = false;
        }

        // - 카메라가 쳐다보는 방향으로 변환한다. (World -> Local)
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity; // 중력 적용

        // 3. 방향으로 이동시키기 
        // 방향을 따라 강제 이동
        // transform.position += direction * _moveSpeed * Time.deltaTime;
        // Controller가 물리 법칙을 적용하여 이동
        _controller.Move(direction * _playerSpeed * Time.deltaTime);
    }

    private void JumpPlayer()
    {
        if (_controller.isGrounded)
        {
            _jumpCount = 0;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (_jumpCount == 0 && _controller.isGrounded)
            {
                _yVelocity = _jumpPower;
                _jumpCount = 1;
            }
            else if (_jumpCount == 1 && _currentStamina > _doubleJumpCost)
            {
                _yVelocity = _jumpPower;
                _jumpCount = 2;
                ConsumeStamina(_doubleJumpCost);
            }
        }
    }
    private void StaminaPlayer()
    {
        if (_isSprinting)
        {
            ConsumeStamina(_sprintCost * Time.deltaTime);
        }

        if (!_isSprinting && (Time.time - _lastStaminaUseTime) >= _gainStaminaDelay)
        {
            RegenerateStamina(_gainStaminaAmount * Time.deltaTime);
        }
    }
    private void RegenerateStamina(float amount)
    {
        _currentStamina = Mathf.Min(_maxStamina, _currentStamina + amount);
    }
    private void ConsumeStamina(float cost)
    {
        _currentStamina = Mathf.Max(0f, _currentStamina - cost);
        _lastStaminaUseTime = Time.time;
    }
}
