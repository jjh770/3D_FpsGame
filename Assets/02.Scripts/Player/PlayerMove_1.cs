using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
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

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        _yVelocity += _config.Gravity * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();

        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _yVelocity = _stats.JumpPower.Value;
        }

        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity; // 중력 적용

        float moveSpeed = _stats.MoveSpeed.Value;
        if (Input.GetKey(KeyCode.LeftShift) && _stats.Stamina.TryConsume(_config.RunStamina * Time.deltaTime))
        {
            moveSpeed = _stats.SprintSpeed.Value;
        }
        _controller.Move(direction * moveSpeed * Time.deltaTime);
    }
}
