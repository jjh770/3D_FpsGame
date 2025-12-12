using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GravityController : MonoBehaviour
{
    [SerializeField] private GravityConfigSO _gravityConfig;
    private CharacterController _controller;
    private float _yVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void UpdateGravity()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _yVelocity < 0)
        {
            _yVelocity = -1f;
        }
        else
        {
            _yVelocity += _gravityConfig.Gravity * Time.deltaTime;
        }
    }

    public float YVelocity => _yVelocity;
    public void SetYVelocity(float value)
    {
        _yVelocity = value;
    }
}
