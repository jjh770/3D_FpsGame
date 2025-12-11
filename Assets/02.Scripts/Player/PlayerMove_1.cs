using System;
using UnityEngine;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다
public class PlayerMove_1 : MonoBehaviour
{
    [Serializable] // json 혹은 ScriptableObject 혹은 DB에서 읽어오게 하면 됨
    public class MoveConfig
    {
        public float Gravity;
        public float RunStamina;
        public float JumpStamina;
    }
    public MoveConfig _config;

    private CharacterController _controller;
    private PlayerStats _stats;
    private float _yVelocity = 0f;
    private int _jumpCount = 0;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        ApplyGravity();
        MoveAction();
        JumpAction();
    }
    private void JumpAction()
    {
        if (_controller.isGrounded)
        {
            _jumpCount = 0;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (_jumpCount == 0 && _controller.isGrounded)
            {
                _yVelocity = _stats.JumpPower.Value;
                _jumpCount = 1;
            }
            else if (_jumpCount == 1 && _stats.Stamina.TryConsume(_config.JumpStamina))
            {
                _yVelocity = _stats.JumpPower.Value;
                _jumpCount = 2;
            }
        }
    }
    private void MoveAction()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();
        bool isMoving = direction.magnitude > 0.1f ? true : false;

        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity; // 중력 적용

        float moveSpeed = _stats.MoveSpeed.Value;
        if (Input.GetKey(KeyCode.LeftShift) && isMoving && _stats.Stamina.TryConsume(_config.RunStamina * Time.deltaTime))
        {
            moveSpeed = _stats.SprintSpeed.Value;
        }
        _controller.Move(direction * moveSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _yVelocity += _config.Gravity * Time.deltaTime;
    }
}
