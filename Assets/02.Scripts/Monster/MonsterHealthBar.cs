using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Monster))]
public class MonsterHealthBar : MonoBehaviour
{
    private Monster _monster;
    private MonsterStats _stats;
    [SerializeField] private Image _gaugeImage;
    [SerializeField] private Transform _healthBarTransform;

    private float _lastHealth = -1;
    private Camera _mainCamera;

    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _stats = GetComponent<MonsterStats>();
        _mainCamera = Camera.main;
    }

    // LateUpdate() 유니티 내부 계산이 끝난 뒤 '화면 갱신'을 위한 코드만 모여있을 때 주로 사용
    private void LateUpdate()
    {
        // UI는 변경사항이 있을 경우에 계속 다시 처음부터 그리기 때문에 예외처리
        // UI 가 알고있는 몬스터 체력값과 다를 때만 fillAmount를 수정한다.
        if (_lastHealth != _stats.Health.Value)
        {
            _lastHealth = _stats.Health.Value;
            _gaugeImage.fillAmount = _stats.Health.Value / _stats.Health.MaxValue;
        }

        // 빌보드 기법 : 카메라의 위치와 회전에 상관없이 항상 정면을 바라보게 하는 기법
        _healthBarTransform.forward = _mainCamera.transform.forward;
    }
}
