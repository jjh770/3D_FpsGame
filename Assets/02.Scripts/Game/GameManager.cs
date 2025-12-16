using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private Player _player;
    [SerializeField] private TextMeshProUGUI _stateTextUI;
    private EGameState _state = EGameState.Ready;
    public event Action<EGameState> OnStateChanged;
    public EGameState State
    {
        get => _state;
        private set
        {
            if (_state != value)
            {
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }
    }

    [SerializeField] private float _readyToPlayTime = 2f;
    [SerializeField] private float _playDelayTime = 0.5f;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        if (_player != null)
        {
            _player.OnPlayerDeathComplete += HandlePlayerDeath;
        }
        SetState(EGameState.Ready);
        StartCoroutine(StartToPlay_Coroutine());
        Cursor.lockState = CursorLockMode.Confined; // 창 내부로 제한
        Cursor.visible = true; // 커서 표시
    }

    public void SetState(EGameState newState)
    {
        _state = newState;
        HandleGameState();
    }

    private void HandleGameState()
    {
        switch (_state)
        {
            case EGameState.Ready:
                Ready();
                break;
            case EGameState.Playing:
                Playing();
                break;
            case EGameState.GameOver:
                GameOver();
                break;
        }
    }

    private void Ready()
    {
        _stateTextUI.gameObject.SetActive(true);
        _stateTextUI.text = "준비중..";
    }

    private void Playing()
    {
        _stateTextUI.gameObject.SetActive(false);
    }

    private void GameOver()
    {
        _stateTextUI.gameObject.SetActive(true);
        _stateTextUI.text = "Game Over";
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        yield return new WaitForSeconds(_readyToPlayTime);
        _stateTextUI.text = "시작!";
        yield return new WaitForSeconds(_playDelayTime);
        SetState(EGameState.Playing);
    }

    private void HandlePlayerDeath()
    {
        SetState(EGameState.GameOver);
    }
}
