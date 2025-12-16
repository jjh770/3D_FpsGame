using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _fpsTransform;
    [SerializeField] private Transform _tpsTransform;
    [SerializeField] private Player _player;

    private bool _isTps;
    private bool _isChanging;
    private bool _canRotateCamera = true;

    private void OnEnable()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath -= HandleDeath;
        }
    }

    private void LateUpdate()
    {
        if (!(GameManager.Instance.State == EGameState.Playing)) return;

        if (!_canRotateCamera) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            _isTps = !_isTps;
        }
        CameraView();
    }

    private void CameraView()
    {
        if (_isChanging) return;

        if (_isTps)
        {
            transform.position = _tpsTransform.position;
            transform.LookAt(_cameraTarget);
        }
        else
        {
            transform.position = _fpsTransform.position;
        }
    }
    private void HandleDeath()
    {
        _canRotateCamera = false;
        Debug.Log("CameraFollow: 시점 전환 비활성화");
    }
}
