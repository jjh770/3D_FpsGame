using System.Collections;
using UnityEngine;

// 카메라 회전 기능
// 마우스를 조작하면 카메라를 그 방향으로 회전하고 싶다
public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Player _player;
    public float RotationSpeed = 200f; // 0 ~ 360;

    // 유니티는 0~360도 각도 체계이므로 우리가 따로 저장할 -360 ~ 360 체계로 누적할 변수
    private float _accumulationX = 0;
    private float _accumulationY = 0;

    [SerializeField] private float _reboundSmoothness = 10f; // 반동 적용 부드러움

    private float _reboundX = 0;
    private float _reboundY = 0;
    private bool _canRotate;

    [Header("Death Camera Settings")]
    [SerializeField] private float _deathCameraHeight = 3f; // 올라갈 높이
    [SerializeField] private float _deathCameraDuration = 2f; // 이동 시간
    [SerializeField] private float _deathCameraDistance = 2f; // 뒤로 물러날 거리

    private bool _isDeathCamera = false;
    private Camera _camera;

    private void Start()
    {
        _canRotate = true;
        _camera = GetComponent<Camera>();
    }
    private void OnEnable()
    {
        if (WeaponEventChannelSO.Instance != null)
        {
            WeaponEventChannelSO.Instance.OnReboundCamera += AddRebound; // 이벤트 구독
        }
        _player.OnPlayerDeath += HandleDeath;
    }
    private void OnDisable()
    {
        if (WeaponEventChannelSO.Instance != null)
        {
            WeaponEventChannelSO.Instance.OnReboundCamera -= AddRebound; // 이벤트 해제
        }
        _player.OnPlayerDeath -= HandleDeath;
    }
    private void Update()
    {
        if (!(GameManager.Instance.State == EGameState.Playing)) return;
        if (!_canRotate) return;
        MouseInput();
        ApplyRotation();
        RecoverRebound();
    }
    private void LateUpdate()
    {
        if (_isDeathCamera && _player != null)
        {
            transform.LookAt(_player.transform.position);
        }
    }

    private void MouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;
        _accumulationY += mouseY * RotationSpeed * Time.deltaTime;

        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);
    }

    private void ApplyRotation()
    {
        // 마우스 회전에 반동을 더함
        float finalX = _accumulationX + _reboundY; // 좌우 반동
        float finalY = _accumulationY + _reboundX; // 상하 반동

        // 상하 회전 최종 제한
        finalY = Mathf.Clamp(finalY, -90f, 90f);

        transform.eulerAngles = new Vector3(-finalY, finalX, 0f);
    }

    private void RecoverRebound()
    {
        // 반동을 0으로 부드럽게 복구
        _reboundX = Mathf.Lerp(_reboundX, 0f, _reboundSmoothness * Time.deltaTime);
        _reboundY = Mathf.Lerp(_reboundY, 0f, _reboundSmoothness * Time.deltaTime);
    }

    private void AddRebound(Vector3 weaponRebound)
    {
        //_accumulationY += weaponRebound.x;
        //_accumulationX += weaponRebound.y;

        _reboundX += weaponRebound.x;
        _reboundY += weaponRebound.y;
    }

    private void HandleDeath()
    {
        _canRotate = false;
        _camera.cullingMask |= (1 << LayerMask.NameToLayer("Player"));
        StartCoroutine(DeathCameraAnimation());
    }
    private IEnumerator DeathCameraAnimation()
    {
        _isDeathCamera = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // 목표 위치: 위로 올라가고 뒤로 물러남
        Vector3 targetOffset = new Vector3(0, _deathCameraHeight, -_deathCameraDistance);
        Vector3 targetPosition = startPosition + targetOffset;

        float elapsedTime = 0f;

        while (elapsedTime < _deathCameraDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _deathCameraDuration;

            // 부드러운 이동 (EaseOut 효과)
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            // 위치 보간
            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // 플레이어를 향해 부드럽게 회전
            if (_player != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_player.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);
            }

            yield return null;
        }

        // 최종 위치와 회전 보장
        transform.position = targetPosition;
    }
}
