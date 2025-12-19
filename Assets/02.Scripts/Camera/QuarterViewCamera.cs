using UnityEngine;

/// <summary>
/// League of Legends 스타일의 회전 가능한 쿼터뷰 카메라
/// - 플레이어를 중앙에 고정하고 주위를 공전
/// - 마우스로 카메라 회전 가능
/// - 고정된 거리와 각도 유지
/// </summary>
public class QuarterViewCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform _target; // 플레이어 Transform

    [Header("Camera Distance & Height")]
    [SerializeField] private float _distance = 12f; // 플레이어와의 거리
    [SerializeField] private float _viewAngle = 45f; // 내려다보는 각도 (도)

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 100f; // 마우스 회전 속도
    [SerializeField] private bool _invertRotation = false; // 회전 반전 여부

    [Header("Smooth Follow")]
    [SerializeField] private float _followSmoothness = 10f; // 따라가기 부드러움 (높을수록 빠름)
    [SerializeField] private float _rotationSmoothness = 8f; // 회전 부드러움

    [Header("Zoom Settings (Optional)")]
    [SerializeField] private bool _enableZoom = true;
    [SerializeField] private float _minDistance = 8f;
    [SerializeField] private float _maxDistance = 20f;
    [SerializeField] private float _zoomSpeed = 2f;

    // 런타임 변수
    private float _currentAngle = 0f; // 현재 수평 회전 각도
    private float _targetAngle = 0f; // 목표 수평 회전 각도
    private Vector3 _velocity = Vector3.zero; // SmoothDamp용

    private void Start()
    {
        if (_target == null)
        {
            Debug.LogError("QuarterViewCamera: Target이 설정되지 않았습니다!");
            return;
        }

        InitializeAngles();
    }

    private void OnEnable()
    {
        if (_target != null)
        {
            InitializeAngles();
        }
    }

    private void InitializeAngles()
    {
        // 초기 각도 설정 (현재 카메라 방향 기준)
        Vector3 direction = transform.position - _target.position;
        _currentAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        _targetAngle = _currentAngle;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        HandleRotationInput();
        HandleZoomInput();
        UpdateCameraPosition();
    }

    private void HandleRotationInput()
    {
        // 마우스 우클릭 드래그로 회전 (LoL 스타일)
        // 또는 항상 마우스 X축으로 회전하려면 조건문 제거
        if (Input.GetMouseButton(1)) // 우클릭 중일 때만 회전
        {
            float mouseX = Input.GetAxis("Mouse X");
            float rotationInput = mouseX * _rotationSpeed * Time.deltaTime;

            if (_invertRotation)
                rotationInput = -rotationInput;

            _targetAngle += rotationInput;
        }

        // 부드러운 회전
        _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, _rotationSmoothness * Time.deltaTime);
    }

    private void HandleZoomInput()
    {
        if (!_enableZoom) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _distance -= scroll * _zoomSpeed;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }
    }

    private void UpdateCameraPosition()
    {
        // 쿼터뷰 각도를 라디안으로 변환
        float angleRad = _viewAngle * Mathf.Deg2Rad;

        // 수평 회전 각도를 라디안으로 변환
        float horizontalRad = _currentAngle * Mathf.Deg2Rad;

        // 쿼터뷰 각도를 고려한 실제 거리 계산
        float horizontalDistance = _distance * Mathf.Cos(angleRad);
        float verticalDistance = _distance * Mathf.Sin(angleRad);

        // 목표 위치 계산 (플레이어 주위를 공전)
        Vector3 offset = new Vector3(
            Mathf.Sin(horizontalRad) * horizontalDistance,
            verticalDistance,
            Mathf.Cos(horizontalRad) * horizontalDistance
        );

        Vector3 targetPosition = _target.position + offset;

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            1f / _followSmoothness
        );

        // 항상 플레이어를 바라봄
        transform.LookAt(_target.position);
    }

    // 에디터에서 기즈모로 카메라 위치 시각화
    private void OnDrawGizmosSelected()
    {
        if (_target == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_target.position, 0.5f);

        // 카메라 위치 예상선
        float angleRad = _viewAngle * Mathf.Deg2Rad;
        float horizontalRad = _currentAngle * Mathf.Deg2Rad;
        float horizontalDistance = _distance * Mathf.Cos(angleRad);
        float verticalDistance = _distance * Mathf.Sin(angleRad);

        Vector3 cameraPos = _target.position + new Vector3(
            Mathf.Sin(horizontalRad) * horizontalDistance,
            verticalDistance,
            Mathf.Cos(horizontalRad) * horizontalDistance
        );

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_target.position, cameraPos);
        Gizmos.DrawWireSphere(cameraPos, 0.3f);
    }
}
