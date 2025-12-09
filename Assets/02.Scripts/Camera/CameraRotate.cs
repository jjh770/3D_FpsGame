using UnityEngine;

// 카메라 회전 기능
// 마우스를 조작하면 카메라를 그 방향으로 회전하고 싶다
public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 200f; // 0 ~ 360;

    // 유니티는 0~360도 각도 체계이므로 우리가 따로 저장할 -360 ~ 360 체계로 누적할 변수
    private float _accumulationx = 0;
    private float _accumulationY = 0;

    // 플레이 눌렀을 때 로딩하는 시간부터 Update는 이미 돌아가고 있음.
    // 그래서 GetMouseButton을 넣어 마우스 위치에 따라 회전이 튀는 현상을 방지
    private void Update()
    {
        // 게임 시작하면 y축이 0도 -> -1도
        if (!Input.GetMouseButton(1))
        {
            return;
        }

        // 1. 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. 마우스 입력을 누적한 방향을 구한다..
        _accumulationx += mouseX * RotationSpeed * Time.deltaTime;
        _accumulationY += mouseY * RotationSpeed * Time.deltaTime;

        // 3. 사람처럼 -90 ~ 90도 사이로 제한한다.
        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);

        // 4. 회전 방향으로 카메라 회전하기
        // 새로운 위치 = 이전 위치 + (속도 * 방향 * 시간)
        // 새로운 회전 = 이전 회전 + (속도 * 방향 * 시간)
        // 왜 (-y축, x축)을 방향으로 설정했는지 eulerAngle에 대해 학습
        transform.eulerAngles = new Vector3(-_accumulationY, _accumulationx);

        // 쿼터니언 : 사원수 -> 쓰는 이유는 짐벌락 현상 방지
        // 학습 : 짐벌락, 쿼터니언을 왜 쓰는가 (게임 수학/물리)
    }
}
