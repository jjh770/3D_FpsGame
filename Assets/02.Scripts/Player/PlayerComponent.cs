using UnityEngine;

public abstract class PlayerComponent : MonoBehaviour
{
    protected Player _player;
    protected bool _isActive = true;

    protected virtual void Awake()
    {
        _player = GetComponent<Player>();
    }

    protected virtual void OnEnable()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath += HandlePlayerDeath;
        }
    }

    protected virtual void OnDisable()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath -= HandlePlayerDeath;
        }
    }

    protected virtual void HandlePlayerDeath()
    {
        _isActive = false;
        Debug.Log($"{GetType().Name}: 플레이어 사망으로 비활성화");
    }

    protected bool CanExecute()
    {
        return _isActive && GameManager.Instance.State == EGameState.Playing;
    }
}
