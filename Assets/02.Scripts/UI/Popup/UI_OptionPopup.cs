using UnityEngine;
using UnityEngine.UI;

public class UI_OptionPopup : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _exitButton;

    // 자주 열리는건 미리 생성해두고 On/Off 하는 방식
    // 아주 가끔씩 열리는 팝업은 프리팹화 후 Instantiate
    private void Start()
    {
        Hide();
        // onclick의 중복 방지 AddListener
        // 콜백함수 : 어떤 이벤트가 일어나면 자동으로 호출되는 함수 (AddListener)
        _continueButton.onClick.AddListener(GameContinue);
        _retryButton.onClick.AddListener(GameRestart);
        _exitButton.onClick.AddListener(GameExit);
    }
    // 함수는 한가지 기능만 해야하고, 그 기능이 무엇을 하는지 (의도, 결과)가 나타나는 이름을 가져야함.
    // 클릭했을 떄 라는 이름은 기능의 이름이 아니라 "언제 호출되는지" 가 드러나 있음.
    private void GameContinue()
    {
        GameManager.Instance.Continue();
        Hide();
    }
    private void GameRestart()
    {
        // UI는 중요한 (도메인/비즈니스)게임 로직을 실행하지 않는다.
        // UI는 (매니저와의) 표현과 통신을 위한 수단일 뿐이다.

        // 인벤토리 UI에서 정렬(이름순, 업데이트순, 공격력 순)
        // 정렬 버튼을 누르면 정렬알고리즘에 의해 정렬이 될것임.
        // 정렬 알고리즘은 UI에 있어야한다? 아이템 매니저(인벤토리)에 있어야한다?

        // 퀘스트 데이터에서 완료 여부를 true/false를 저장함.
        // UI는 퀘스트 매니저에서 데이터가 가져다가 완료/미완료를 표현함.
        GameManager.Instance.Restart();
    }
    private void GameExit()
    {
        GameManager.Instance.Quit();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
