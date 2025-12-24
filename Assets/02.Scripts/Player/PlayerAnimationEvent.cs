using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerBombFire _playerBombFire;
    private PlayerMove _playerMove;

    private void Awake()
    {
        _playerBombFire = GetComponentInParent<PlayerBombFire>();
        _playerMove = GetComponentInParent<PlayerMove>();
    }

    public void OnThrowing()
    {
        _playerBombFire.ThrowBomb();
    }

    public void FinishThrowing()
    {
        _playerBombFire.FinishThrowBomb();
    }

    public void FinishJumping()
    {
        _playerMove.FinishJumpAnimation();
    }
}
