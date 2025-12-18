using UnityEngine;

public enum ECameraMode
{
    FPS,
    TPS
}

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _fpsTransform;
    [SerializeField] private Transform _tpsTransform;
    [SerializeField] private Player _player;

    [Header("Camera Mode Settings")]
    [SerializeField] private CameraRotate _cameraRotate;
    [SerializeField] private ECameraMode _currentMode = ECameraMode.FPS;

    [Header("TPS View Settings")]
    [SerializeField] private float _tpsDistance = 1.2f; // 플레이어와의 거리
    [SerializeField] private float _tpsHeightOffset = 0.7f; // 플레이어 어깨/눈 높이
    [SerializeField] private Vector3 _tpsShoulderOffset = new Vector3(0.45f, 0f, 0f); // 어깨 너머 오프셋 (우측)
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private float _followSmoothness = 10f;
    [SerializeField] private float _rotationSmoothness = 8f;
    [SerializeField] private float _minPitch = -45f; // 최소 수직 각도 (아래)
    [SerializeField] private float _maxPitch = 45f; // 최대 수직 각도 (위)

    private bool _isChanging;
    private bool _canRotateCamera = true;
    private Camera _camera;

    // TPS View 런타임 변수
    private float _currentYaw = 0f; // 수평 회전
    private float _targetYaw = 0f;
    private float _currentPitch = 0f; // 수직 회전
    private float _targetPitch = 0f;
    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (_player != null)
        {
            _player.OnPlayerDeath += HandleDeath;
        }

        // 시작 시 모드 설정
        SwitchCameraMode(_currentMode);
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
            ToggleCameraMode();
        }

        // 현재 모드에 따라 카메라 업데이트
        if (_currentMode == ECameraMode.TPS)
        {
            HandleTPSInput();
            UpdateTPSCamera();
        }
        else
        {
            CameraView();
        }
    }

    private void ToggleCameraMode()
    {
        _currentMode = _currentMode == ECameraMode.FPS
            ? ECameraMode.TPS
            : ECameraMode.FPS;

        SwitchCameraMode(_currentMode);
    }

    private void SwitchCameraMode(ECameraMode mode)
    {
        switch (mode)
        {
            case ECameraMode.FPS:
                // FPS 모드: CameraRotate 활성화
                if (_cameraRotate != null) _cameraRotate.enabled = true;
                _camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
                Debug.Log("카메라 모드: FPS");
                break;

            case ECameraMode.TPS:
                // TPS 모드: CameraRotate 비활성화
                if (_cameraRotate != null) _cameraRotate.enabled = false;

                _camera.cullingMask |= (1 << LayerMask.NameToLayer("Player"));
                InitializeTPSAngles();
                Debug.Log("카메라 모드: TPS");
                break;
        }
    }

    private void InitializeTPSAngles()
    {
        if (_player == null) return;

        // 플레이어 뒤쪽에서 시작 (배틀그라운드 스타일)
        _currentYaw = _player.transform.eulerAngles.y;
        _targetYaw = _currentYaw;
        _currentPitch = 10f; // 약간 위에서 시작
        _targetPitch = _currentPitch;
    }

    private void HandleTPSInput()
    {
        // 마우스로 카메라 회전 (배틀그라운드 스타일)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _targetYaw += mouseX * _rotationSpeed * Time.deltaTime;
        _targetPitch += mouseY * _rotationSpeed * Time.deltaTime;

        // 수직 각도 제한
        _targetPitch = Mathf.Clamp(_targetPitch, _minPitch, _maxPitch);

        // 부드러운 회전
        _currentYaw = Mathf.LerpAngle(_currentYaw, _targetYaw, _rotationSmoothness * Time.deltaTime);
        _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, _rotationSmoothness * Time.deltaTime);
    }

    private void UpdateTPSCamera()
    {
        if (_player == null) return;

        // 플레이어를 카메라 수평 방향으로 회전 (카메라가 보는 방향 = 플레이어가 보는 방향)
        _player.transform.rotation = Quaternion.Euler(0, _currentYaw, 0);

        // 플레이어 어깨/눈 높이 기준점
        Vector3 playerPivot = _player.transform.position + Vector3.up * _tpsHeightOffset;

        // 수평/수직 각도를 라디안으로 변환
        float yawRad = _currentYaw * Mathf.Deg2Rad;
        float pitchRad = _currentPitch * Mathf.Deg2Rad;

        // 카메라 오프셋 계산 (플레이어 뒤에서 약간 위/옆)
        float horizontalDistance = _tpsDistance * Mathf.Cos(pitchRad);
        float verticalDistance = _tpsDistance * Mathf.Sin(pitchRad);

        Vector3 cameraOffset = new Vector3(
            Mathf.Sin(yawRad) * horizontalDistance,
            verticalDistance,
            Mathf.Cos(yawRad) * horizontalDistance
        );

        // 어깨 너머 오프셋 적용 (플레이어 회전 기준)
        Quaternion playerRotation = Quaternion.Euler(0, _currentYaw, 0);
        Vector3 shoulderOffset = playerRotation * _tpsShoulderOffset;

        // 최종 카메라 위치
        Vector3 targetPosition = playerPivot - cameraOffset + shoulderOffset;

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            1f / _followSmoothness
        );

        // 어깨 오프셋이 적용된 기준점을 바라봄
        Vector3 lookAtPoint = playerPivot + shoulderOffset;
        transform.LookAt(lookAtPoint);
    }

    private void CameraView()
    {
        if (_isChanging) return;

        // FPS 모드일 때만 사용
        transform.position = _fpsTransform.position;
    }

    private void HandleDeath()
    {
        _canRotateCamera = false;
        Debug.Log("CameraFollow: 시점 전환 비활성화");
    }
}
