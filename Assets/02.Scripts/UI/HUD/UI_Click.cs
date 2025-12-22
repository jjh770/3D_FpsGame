using TMPro;
using UnityEngine;

public class UI_Click : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leftClickCountTextUI;
    [SerializeField] private TextMeshProUGUI _rightClickCountTextUI;

    private void Start()
    {
        // 구독 시작(데이터 변경되면 Refresh 호출해주세요 라고 등록)
        // 구독자들이 등록한 함수를 '콜백 함수'라고 함. (어떤 이벤트가 발생하면 실행되는 함수를 콜백 함수)
        ClickManager.Instance.OnDataChanged += Refresh;
    }

    private void Refresh()
    {
        _leftClickCountTextUI.text = $"왼쪽 클릭 : {ClickManager.Instance.LeftClickCount}번";
        _rightClickCountTextUI.text = $"오른쪽 클릭 : {ClickManager.Instance.RightClickCount}번";
    }
}
