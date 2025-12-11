using UnityEngine;

// 카메라 회전 기능
// 마우스를 조작하면 카메라를 그 방향으로 회전하고 싶다
public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 200f; // 0 ~ 360;

    // 유니티는 0~360도 각도 체계이므로 우리가 따로 저장할 -360 ~ 360 체계로 누적할 변수
    private float _accumulationX = 0;
    private float _accumulationY = 0;

    [SerializeField] private float _reboundSmoothness = 10f; // 반동 적용 부드러움

    private float _reboundX = 0;
    private float _reboundY = 0;
    private void OnEnable()
    {
        WeaponEvents.OnReboundCamera += AddRebound; // 이벤트 구독
    }
    private void OnDisable()
    {
        WeaponEvents.OnReboundCamera -= AddRebound; // 이벤트 해제
    }
    private void Update()
    {
        MouseInput();
        ApplyRotation();
        RecoverRebound();
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
}
