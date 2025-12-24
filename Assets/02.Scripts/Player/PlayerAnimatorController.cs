using UnityEngine;

public class PlayerAnimatorController : PlayerComponent
{
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
    }

    public void MoveAnimation(bool isMoving)
    {
        _animator.SetBool("IsMoving", isMoving);
    }

    public void JumpAnimation()
    {
        _animator.SetTrigger("Jump");
    }

    public void DeathAnimation()
    {
        _animator.SetTrigger("Death");
    }

    public void ThrowAnimation()
    {
        _animator.SetTrigger("Throw");
    }
}
