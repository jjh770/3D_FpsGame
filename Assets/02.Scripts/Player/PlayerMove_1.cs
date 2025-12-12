using UnityEngine;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다
public class PlayerMove_1 : MonoBehaviour
{
    [SerializeField] private CharacterMoveConfigSO _moveConfig;
    private CharacterController _controller;
    private GravityController _gravityController;
    private PlayerStats _stats;
    private int _jumpCount = 0;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _gravityController = GetComponent<GravityController>();
        _stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        _gravityController.UpdateGravity();
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
                _gravityController.SetYVelocity(_stats.JumpPower.Value);
                _jumpCount = 1;
            }
            else if (_jumpCount == 1 && _stats.Stamina.TryConsume(_moveConfig.JumpStamina))
            {
                _gravityController.SetYVelocity(_stats.JumpPower.Value);
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
        bool isMoving = direction.magnitude > 0.1f;

        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _gravityController.YVelocity;

        float moveSpeed = _stats.MoveSpeed.Value;
        if (Input.GetKey(KeyCode.LeftShift) && isMoving && _stats.Stamina.TryConsume(_moveConfig.RunStamina * Time.deltaTime))
        {
            moveSpeed = _stats.SprintSpeed.Value;
        }
        _controller.Move(direction * moveSpeed * Time.deltaTime);
    }
}
