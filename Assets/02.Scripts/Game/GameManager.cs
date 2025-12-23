using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private Player _player;
    [SerializeField] private TextMeshProUGUI _stateTextUI;
    [SerializeField] private UI_OptionPopup _optionPopup;
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
                HandleGameState();
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
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    private void Pause()
    {
        UnlockCursor();
        Time.timeScale = 0f;
        _optionPopup.Show();
    }
    public void Continue()
    {
        LockCursor();
        Time.timeScale = 1f;
        _optionPopup.Hide();
    }
    public void Restart()
    {
        LockCursor();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif    
    }
    public void SetState(EGameState newState)
    {
        State = newState;
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

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
