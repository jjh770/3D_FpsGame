using UnityEngine;

public abstract class PlayerComponent : MonoBehaviour
{
    protected Player _player;
    protected bool _isActive = true;
    private bool _isGamePlaying = true;

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;
            // 현재 상태로 초기화
            HandleGameStateChanged(GameManager.Instance.State);
        }
    }

    protected virtual void OnDisable()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath -= HandlePlayerDeath;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }
    }

    protected virtual void HandlePlayerDeath()
    {
        _isActive = false;
        Debug.Log($"{GetType().Name}: 플레이어 사망으로 비활성화");
    }

    protected virtual void HandleGameStateChanged(EGameState newState)
    {
        _isGamePlaying = (newState == EGameState.Playing);
    }

    protected bool CanExecute()
    {
        return _isActive && GameManager.Instance.State == EGameState.Playing;
    }
}
